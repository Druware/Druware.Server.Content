using System.Text.Json;
using System.Text.Json.Serialization;
using Druware.Server.Content.Entities;
using Druware.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Druware.Server.Content.UnitTests;

public class ReviewTests
{
    private static readonly DateTime Stamp = new(2026, 10, 15, 9, 30, 0);

    // the host serialises with ReferenceHandler.Preserve
    private static readonly JsonSerializerOptions Preserve =
        new(JsonSerializerDefaults.Web)
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

    private static Review NewReview(string permalink = "best-tube",
        bool isPublished = true, DateTime? posted = null) =>
        new()
        {
            Title = "The best tube",
            Permalink = permalink,
            ProductName = "Tubolito S-Tubo",
            IsPublished = isPublished,
            Posted = posted ?? Stamp,
            Modified = posted ?? Stamp
        };

    // a valid review, with every optional field empty
    private static Review Minimal() =>
        new() { Title = "The best tube", ProductName = "Tubolito S-Tubo" };

    // the tag table that ReviewTag joins belongs to ServerContext, so create
    // it first: EnsureCreated does nothing once the database has a table
    private static ContentContextSqlite CreateMigratedContextWithTags(
        SqliteDatabase database)
    {
        using (var server = new ServerContext(
                   new DbContextOptionsBuilder<ServerContext>()
                       .UseSqlite(database.Connection)
                       .Options, SqliteConfiguration))
            server.Database.EnsureCreated();
        return database.CreateMigratedContentContext();
    }

    private static readonly IConfiguration SqliteConfiguration =
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["API:Database:ConnectionType"] = "Sqlite",
                ["API:Database:ConnectionString"] = "DataSource=:memory:"
            })
            .Build();

    private static Asset NewAsset() =>
        new()
        {
            Description = "A tube on a bench",
            FileName = "tube.jpg",
            MediaType = "image/jpeg",
            Content = [1, 2, 3],
            Type = new AssetType { Description = "Image" }
        };

    private static void AddTags(Review review, params string[] names)
    {
        foreach (var name in names)
            review.ReviewTags.Add(new ReviewTag { Tag = new Tag { Name = name } });
    }

    // ---- persistence ------------------------------------------------------

    [Fact]
    public void EveryFieldRoundTrips()
    {
        using var database = new SqliteDatabase();
        long photoId;
        using (var context = CreateMigratedContextWithTags(database))
        {
            var asset = NewAsset();
            context.Assets!.Add(asset);
            context.SaveChanges();
            photoId = asset.AssetId;

            var review = new Review
            {
                Title = "The best tube",
                Permalink = "best-tube",
                ProductName = "Tubolito S-Tubo",
                Category = "Tubes",
                Summary = "Light, small, and it holds air.",
                Body = "# Verdict\n\nBuy two.",
                PhotoAssetId = photoId,
                ProductUrl = "https://example.com/tubolito?ref=twj",
                IsAffiliate = true,
                EddieScore = 9,
                DruScore = 7,
                EddieTake = "Ride it.",
                DruTake = "Carry a spare.",
                Posted = Stamp,
                Modified = Stamp.AddHours(1),
                IsPublished = true,
                IsFeatured = true
            };
            AddTags(review, "tubes", "gear");
            context.Reviews!.Add(review);
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();
        var stored = Review.ByPermalinkOrId(reader, "best-tube")!;

        Assert.NotNull(stored.ReviewId);
        Assert.Equal("The best tube", stored.Title);
        Assert.Equal("best-tube", stored.Permalink);
        Assert.Equal("Tubolito S-Tubo", stored.ProductName);
        Assert.Equal("Tubes", stored.Category);
        Assert.Equal("Light, small, and it holds air.", stored.Summary);
        Assert.Equal("# Verdict\n\nBuy two.", stored.Body);
        Assert.Equal(photoId, stored.PhotoAssetId);
        Assert.Equal("https://example.com/tubolito?ref=twj", stored.ProductUrl);
        Assert.True(stored.IsAffiliate);
        Assert.Equal(9, stored.EddieScore);
        Assert.Equal(7, stored.DruScore);
        Assert.Equal("Ride it.", stored.EddieTake);
        Assert.Equal("Carry a spare.", stored.DruTake);
        Assert.Equal(Stamp, stored.Posted);
        Assert.Equal(Stamp.AddHours(1), stored.Modified);
        Assert.True(stored.IsPublished);
        Assert.True(stored.IsFeatured);
        Assert.Equal(new[] { "gear", "tubes" }, stored.Tags!.Order().ToArray());
    }

    [Fact]
    public void OptionalFieldsAreEmptyAndFlagsAreFalseWhenNotSet()
    {
        using var database = new SqliteDatabase();
        using (var context = CreateMigratedContextWithTags(database))
        {
            context.Reviews!.Add(Minimal());
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();
        var stored = reader.Reviews!.Include("ReviewTags.Tag").Single();

        Assert.Null(stored.Permalink);
        Assert.Null(stored.Category);
        Assert.Null(stored.Summary);
        Assert.Null(stored.Body);
        Assert.Null(stored.PhotoAssetId);
        Assert.Null(stored.ProductUrl);
        Assert.False(stored.IsAffiliate);
        Assert.Null(stored.EddieScore);
        Assert.Null(stored.DruScore);
        Assert.Null(stored.EddieTake);
        Assert.Null(stored.DruTake);
        Assert.Null(stored.Posted);
        Assert.Null(stored.Modified);
        Assert.False(stored.IsPublished);
        Assert.False(stored.IsFeatured);
        Assert.Empty(stored.Tags!);
    }

    [Fact]
    public void DuplicatePermalinkIsRejectedByTheDatabase()
    {
        using var database = new SqliteDatabase();
        using (var context = database.CreateMigratedContentContext())
        {
            context.Reviews!.Add(NewReview("best-tube"));
            context.SaveChanges();
        }

        using var second = database.CreateContentContext();
        second.Reviews!.Add(NewReview("best-tube"));

        Assert.Throws<DbUpdateException>(() => second.SaveChanges());
    }

    [Fact]
    public void ManyReviewsMayHaveNoPermalink()
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateMigratedContentContext();
        context.Reviews!.AddRange(Minimal(), Minimal());
        context.SaveChanges();

        Assert.Equal(2, context.Reviews!.Count());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)] // only the database's cascade can remove them
    public void DeletingAReviewDeletesItsTagRows(bool loadTags)
    {
        using var database = new SqliteDatabase();
        using (var context = CreateMigratedContextWithTags(database))
        {
            var review = NewReview();
            AddTags(review, "tubes", "gear");
            context.Reviews!.Add(review);
            context.SaveChanges();
        }

        using (var context = database.CreateContentContext())
        {
            Assert.Equal(2, context.ReviewTags!.Count());
            var review = loadTags
                ? context.Reviews!.Include("ReviewTags").Single()
                : context.Reviews!.Single();
            context.Reviews!.Remove(review);
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();
        Assert.Empty(reader.Reviews!);
        Assert.Empty(reader.ReviewTags!);
    }

    [Fact]
    public void DeletingThePhotoAssetLeavesTheReviewWithoutAPhoto()
    {
        using var database = new SqliteDatabase();
        using (var context = database.CreateMigratedContentContext())
        {
            var asset = NewAsset();
            context.Assets!.Add(asset);
            context.SaveChanges();

            var review = NewReview();
            review.PhotoAssetId = asset.AssetId;
            context.Reviews!.Add(review);
            context.SaveChanges();
        }

        // the review is not loaded, so the database does the work
        using (var context = database.CreateContentContext())
        {
            context.Assets!.Remove(context.Assets!.Single());
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();
        var stored = reader.Reviews!.Single();
        Assert.Null(stored.PhotoAssetId);
        Assert.Empty(reader.Assets!);
    }

    // ---- ByPermalinkOrId --------------------------------------------------

    [Fact]
    public void ByPermalinkOrIdFindsByIdAndByPermalink()
    {
        using var database = new SqliteDatabase();
        using var context = CreateMigratedContextWithTags(database);
        var first = NewReview("best-tube");
        AddTags(first, "tubes");
        var second = NewReview("best-light");
        context.Reviews!.AddRange(first, second);
        context.SaveChanges();

        var byPermalink = Review.ByPermalinkOrId(context, "best-tube");
        Assert.NotNull(byPermalink);
        Assert.Equal(first.ReviewId, byPermalink.ReviewId);
        Assert.Equal(new[] { "tubes" }, byPermalink.Tags);

        var byId = Review.ByPermalinkOrId(context, second.ReviewId!.Value.ToString());
        Assert.NotNull(byId);
        Assert.Equal("best-light", byId.Permalink);
    }

    [Theory]
    [InlineData("no-such-review")]
    [InlineData("999")]
    [InlineData("")]
    public void ByPermalinkOrIdReturnsNullWhenNothingMatches(string value)
    {
        using var database = new SqliteDatabase();
        using var context = CreateMigratedContextWithTags(database);
        context.Reviews!.Add(NewReview("best-tube"));
        context.SaveChanges();

        Assert.Null(Review.ByPermalinkOrId(context, value));
    }

    [Fact]
    public void ByPermalinkOrIdHidesADraftUnlessAsked()
    {
        using var database = new SqliteDatabase();
        using var context = CreateMigratedContextWithTags(database);
        var draft = NewReview("draft", isPublished: false);
        context.Reviews!.Add(draft);
        context.SaveChanges();
        var id = draft.ReviewId!.Value.ToString();

        Assert.Null(Review.ByPermalinkOrId(context, "draft", includeUnpublished: false));
        Assert.Null(Review.ByPermalinkOrId(context, id, includeUnpublished: false));
        Assert.NotNull(Review.ByPermalinkOrId(context, "draft"));
        Assert.NotNull(Review.ByPermalinkOrId(context, id, includeUnpublished: true));
    }

    // ---- permalinks -------------------------------------------------------

    [Fact]
    public void IsPermalinkValidRefusesAPermalinkInUseByAnotherReview()
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateMigratedContentContext();
        var review = NewReview("best-tube");
        context.Reviews!.Add(review);
        context.SaveChanges();

        Assert.False(Review.IsPermalinkValid(context, "best-tube"));
        Assert.False(Review.IsPermalinkValid(context, "best-tube", review.ReviewId + 1));
        Assert.True(Review.IsPermalinkValid(context, "best-tube", review.ReviewId));
        Assert.True(Review.IsPermalinkValid(context, "best-light"));
    }

    [Theory]
    [InlineData("The Best Tube", "the_best_tube")]
    [InlineData("Lights, Camera, Action!", "lights_camera_action")]
    [InlineData("Is it \"worth it\"?", "is_it_worth_it")]
    [InlineData("a=b", "ab")]
    [InlineData("Tubes & Tyres", "tubes_%26_tyres")]
    [InlineData("Café", "caf%c3%a9")]
    [InlineData("2024", "2024")]
    public void GeneratePermalinkFollowsTheNewsRule(string title, string expected) =>
        Assert.Equal(expected, Review.GeneratePermalink(title));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EnsurePermalinkFillsAMissingPermalinkFromTheTitle(string? permalink)
    {
        var review = Minimal();
        review.Title = "Lights, Camera, Action!";
        review.Permalink = permalink;

        Review.EnsurePermalink(review);

        Assert.Equal("lights_camera_action", review.Permalink);
    }

    [Fact]
    public void EnsurePermalinkKeepsAPermalinkThatIsSet()
    {
        var review = Minimal();
        review.Permalink = "my-own-link";

        Review.EnsurePermalink(review);

        Assert.Equal("my-own-link", review.Permalink);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void EnsurePermalinkLeavesItToValidateWhenTheTitleIsBlank(string? title)
    {
        var review = Minimal();
        review.Title = title!;

        Review.EnsurePermalink(review);

        Assert.Null(review.Permalink);
        Assert.Equal("Title is required", Review.Validate(review));
    }

    [Fact]
    public void EnsurePermalinkOutputIsCheckedByValidate()
    {
        var review = Minimal();
        review.Title = "2024";

        Review.EnsurePermalink(review);

        Assert.Equal("2024", review.Permalink);
        Assert.Equal("Permalink cannot be only digits", Review.Validate(review));
    }

    [Fact]
    public void ARegeneratedPermalinkEqualToTheReviewsOwnIsValid()
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateMigratedContentContext();
        var review = NewReview(Review.GeneratePermalink("The best tube"));
        context.Reviews!.Add(review);
        context.SaveChanges();

        // what an update with a blank permalink does
        var body = Minimal();
        body.Permalink = "";
        Review.Normalize(body);
        Review.EnsurePermalink(body);

        Assert.Equal(review.Permalink, body.Permalink);
        Assert.True(Review.IsPermalinkValid(context, body.Permalink!, review.ReviewId));
        Assert.False(Review.IsPermalinkValid(context, body.Permalink!));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("123")]
    [InlineData("2024")]
    public void ValidateRejectsAnAllDigitPermalink(string permalink)
    {
        var review = Minimal();
        review.Permalink = permalink;

        Assert.Equal("Permalink cannot be only digits", Review.Validate(review));
    }

    [Theory]
    [InlineData("best-tube")]
    [InlineData("2024-best-tube")]
    [InlineData("tube-2")]
    public void ValidateAcceptsAPermalinkWithMoreThanDigits(string permalink)
    {
        var review = Minimal();
        review.Permalink = permalink;

        Assert.Null(Review.Validate(review));
    }

    // ---- validation -------------------------------------------------------

    [Fact]
    public void ValidateAcceptsAMinimalReview() =>
        Assert.Null(Review.Validate(Minimal()));

    [Theory]
    [InlineData(null, null)]
    [InlineData(1, null)]
    [InlineData(10, null)]
    [InlineData(0, "must be a whole number from 1 to 10")]
    [InlineData(11, "must be a whole number from 1 to 10")]
    [InlineData(-3, "must be a whole number from 1 to 10")]
    public void ValidateChecksBothScores(int? score, string? problem)
    {
        var eddie = Minimal();
        eddie.EddieScore = score;
        var dru = Minimal();
        dru.DruScore = score;

        Assert.Equal(problem == null ? null : "Eddie's score " + problem,
            Review.Validate(eddie));
        Assert.Equal(problem == null ? null : "Dru's score " + problem,
            Review.Validate(dru));
    }

    [Fact]
    public void ValidateChecksTakesWithTheArticleRules()
    {
        var review = Minimal();
        review.EddieTake = new string('e', Article.TakeMaxLength);
        review.DruTake = "  " + new string('d', Article.TakeMaxLength) + "  ";
        Assert.Null(Review.Validate(review));

        review.DruTake = new string('d', Article.TakeMaxLength + 1);
        Assert.Equal("Dru's take must be at most 280 characters",
            Review.Validate(review));

        review.EddieTake = "Ride it.\nThen fix it.";
        Assert.Equal("Eddie's take must be a single line",
            Review.Validate(review));
    }

    [Theory]
    [InlineData("http://example.com")]
    [InlineData("https://example.com/tubolito?ref=twj")]
    [InlineData("HTTPS://EXAMPLE.COM/")]
    public void ValidateAcceptsAnAbsoluteWebProductUrl(string url)
    {
        var review = Minimal();
        review.ProductUrl = url;

        Assert.Null(Review.Validate(review));
    }

    [Theory]
    [InlineData("/gear/tubolito")]
    [InlineData("gear/tubolito")]
    [InlineData("example.com/tubolito")]
    [InlineData("ftp://example.com/tubolito")]
    [InlineData("mailto:sales@example.com")]
    [InlineData("file:///etc/passwd")]
    [InlineData("https://")]
    public void ValidateRejectsAProductUrlThatIsNotAnAbsoluteWebAddress(string url)
    {
        var review = Minimal();
        review.ProductUrl = url;

        Assert.Equal("Product URL must be a full http or https address",
            Review.Validate(review));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateRequiresATitle(string? title)
    {
        var review = Minimal();
        review.Title = title!;

        Assert.Equal("Title is required", Review.Validate(review));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateRequiresAProductName(string? productName)
    {
        var review = Minimal();
        review.ProductName = productName!;

        Assert.Equal("Product name is required", Review.Validate(review));
    }

    public static TheoryData<string, int, Action<Review, string>> MaxLengths => new()
    {
        { "Title", Review.TitleMaxLength, (r, v) => r.Title = v },
        { "Product name", Review.ProductNameMaxLength, (r, v) => r.ProductName = v },
        { "Permalink", Review.PermalinkMaxLength, (r, v) => r.Permalink = v },
        { "Category", Review.CategoryMaxLength, (r, v) => r.Category = v },
        { "Summary", Review.SummaryMaxLength, (r, v) => r.Summary = v },
        {
            "Product URL", Review.ProductUrlMaxLength,
            (r, v) => r.ProductUrl = "https://example.com/" + v[20..]
        }
    };

    [Theory]
    [MemberData(nameof(MaxLengths))]
    public void ValidateEnforcesTheMaximumLengths(string label, int max,
        Action<Review, string> set)
    {
        var review = Minimal();
        set(review, new string('a', max));
        Assert.Null(Review.Validate(review));

        set(review, new string('a', max + 1));
        Assert.Equal($"{label} must be at most {max} characters",
            Review.Validate(review));
    }

    [Fact]
    public void NormalizeTrimsTakesAndBlanksToNull()
    {
        var review = Minimal();
        review.EddieTake = "  Ride it.  ";
        review.DruTake = "   ";
        review.Permalink = "  ";
        review.ProductUrl = "";
        review.Category = "Tubes";

        Review.Normalize(review);

        Assert.Equal("Ride it.", review.EddieTake);
        Assert.Null(review.DruTake);
        Assert.Null(review.Permalink);
        Assert.Null(review.ProductUrl);
        Assert.Equal("Tubes", review.Category);
    }

    // ---- the list ---------------------------------------------------------

    [Fact]
    public void ListLeavesOutDraftsUnlessAskedAndOrdersNewestFirst()
    {
        using var database = new SqliteDatabase();
        using (var context = CreateMigratedContextWithTags(database))
        {
            // added in this order, so the ids rise in this order too
            context.Reviews!.Add(NewReview("older", posted: Stamp.AddDays(-2)));
            context.Reviews!.Add(NewReview("draft", isPublished: false, posted: Stamp.AddDays(1)));
            context.Reviews!.Add(NewReview("same-day-first", posted: Stamp));
            context.Reviews!.Add(NewReview("same-day-second", posted: Stamp));
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();

        Assert.Equal(
            new[] { "same-day-second", "same-day-first", "older" },
            Review.List(reader, includeUnpublished: false)
                .Select(r => r.Permalink).ToArray());
        Assert.Equal(3, Review.List(reader, includeUnpublished: false).Count());

        Assert.Equal(
            new[] { "draft", "same-day-second", "same-day-first", "older" },
            Review.List(reader, includeUnpublished: true)
                .Select(r => r.Permalink).ToArray());
    }

    [Fact]
    public void ListIncludesTheTags()
    {
        using var database = new SqliteDatabase();
        using (var context = CreateMigratedContextWithTags(database))
        {
            var review = NewReview();
            AddTags(review, "tubes");
            context.Reviews!.Add(review);
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();
        var listed = Review.List(reader, includeUnpublished: false).Single();

        Assert.Equal(new[] { "tubes" }, listed.Tags);
    }

    // ---- PostgreSQL timestamps ---------------------------------------------

    // Npgsql maps DateTime to timestamptz by default, which rejects the local
    // times the controllers write; building a model needs no server
    [Theory]
    [InlineData(nameof(Review.Posted))]
    [InlineData(nameof(Review.Modified))]
    public void PostgreSqlStoresTheStampsWithoutTimeZone(string property)
    {
        const string noServer = "Host=localhost;Database=unused";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["API:Database:ConnectionType"] = "PostgreSql",
                ["API:Database:ConnectionString"] = noServer
            })
            .Build();
        using var migration = new ContentContextPostgreSql(
            new DbContextOptionsBuilder<ContentContextPostgreSql>()
                .UseNpgsql(noServer).Options, null);
        using var runtime = new ContentContext(
            new DbContextOptionsBuilder<ContentContext>()
                .UseNpgsql(noServer).Options, configuration);

        foreach (DbContext context in new DbContext[] { migration, runtime })
            Assert.Equal("timestamp without time zone",
                context.Model.FindEntityType(typeof(Review))!
                    .FindProperty(property)!.GetColumnType());
    }

    // ---- the JSON shape ---------------------------------------------------

    [Fact]
    public void SerialisesTagsAsStringsAndNotTheJoinRows()
    {
        var review = NewReview();
        review.EddieScore = 8;
        AddTags(review, "tubes", "gear");

        using var document = JsonDocument.Parse(
            JsonSerializer.Serialize(review, Preserve));
        var root = document.RootElement;

        // Preserve does not wrap a string[] in $id/$values
        var tags = root.GetProperty("tags")
            .EnumerateArray().Select(t => t.GetString()).ToArray();
        Assert.Equal(new[] { "tubes", "gear" }, tags);
        Assert.False(root.TryGetProperty("reviewTags", out _));

        Assert.Equal(
            new[]
            {
                "$id", "reviewId", "title", "permalink", "productName",
                "category", "summary", "body", "photoAssetId", "productUrl",
                "isAffiliate", "eddieScore", "druScore", "eddieTake",
                "druTake", "tags", "posted", "modified", "isPublished",
                "isFeatured"
            },
            root.EnumerateObject().Select(p => p.Name).ToArray());
        Assert.Equal(8, root.GetProperty("eddieScore").GetInt32());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("druScore").ValueKind);
    }

    [Fact]
    public void APreservedResponseReadsBackAsABody()
    {
        var review = NewReview();
        review.EddieTake = "Ride it.";
        AddTags(review, "tubes", "gear");

        var model = JsonSerializer.Deserialize<Review>(
            JsonSerializer.Serialize(review, Preserve), Preserve)!;

        Assert.Equal("best-tube", model.Permalink);
        Assert.Equal("Ride it.", model.EddieTake);
        Assert.Equal(new[] { "tubes", "gear" }, model.Tags);
        Assert.Empty(model.ReviewTags);
        Assert.Null(Review.Validate(model));
    }
}

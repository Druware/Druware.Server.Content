using System.Text.Json;
using System.Text.Json.Serialization;
using Druware.Server.Content.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Druware.Server.Content.UnitTests;

public class ArticleTakesAndMetaTests
{
    private static readonly JsonSerializerOptions Web =
        new(JsonSerializerDefaults.Web);

    // the host serialises with ReferenceHandler.Preserve
    private static readonly JsonSerializerOptions Preserve =
        new(JsonSerializerDefaults.Web)
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

    private static Article NewArticle(string permalink = "two-takes") =>
        new()
        {
            Title = "Two takes",
            Summary = "Summary",
            Body = "Body",
            Permalink = permalink
        };

    private static Dictionary<string, string> Meta(params string[] pairs)
    {
        Dictionary<string, string> meta = new();
        for (var i = 0; i < pairs.Length; i += 2)
            meta[pairs[i]] = pairs[i + 1];
        return meta;
    }

    // saves an article carrying the given meta, returning its id
    private static Guid SaveArticle(SqliteDatabase database,
        Dictionary<string, string> meta)
    {
        using var context = database.CreateMigratedContentContext();
        var article = NewArticle();
        Article.ApplyMeta(context, article, meta);
        context.News!.Add(article);
        context.SaveChanges();
        return article.ArticleId!.Value;
    }

    // the meta rows as stored, read through a fresh context
    private static Dictionary<string, string?> StoredMeta(SqliteDatabase database)
    {
        using var reader = database.CreateContentContext();
        return reader.ArticleMeta!
            .ToDictionary(m => m.Property!, m => m.Value);
    }

    // replaces the meta of the stored article the way NewsController.Update
    // does: read with ByPermalinkOrId's Include, apply, save
    private static void UpdateMeta(SqliteDatabase database,
        Dictionary<string, string>? meta)
    {
        using var context = database.CreateContentContext();
        var article = context.News!.Include("ArticleMeta").Single();
        Article.ApplyMeta(context, article, meta);
        context.SaveChanges();
    }

    // ---- persistence ------------------------------------------------------

    [Fact]
    public void TakesAndMetaRoundTrip()
    {
        using var database = new SqliteDatabase();
        using (var context = database.CreateMigratedContentContext())
        {
            var article = NewArticle();
            article.EddieTake = "Ride it.";
            article.DruTake = "Fix it first.";
            Article.ApplyMeta(context, article, Meta(
                "description", "Two takes on one bike",
                "og:title", "Two takes"));
            context.News!.Add(article);
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();
        var stored = reader.News!.Include("ArticleMeta").Single();

        Assert.Equal("Ride it.", stored.EddieTake);
        Assert.Equal("Fix it first.", stored.DruTake);
        Assert.Equal(Meta(
                "description", "Two takes on one bike",
                "og:title", "Two takes"),
            stored.Meta);
    }

    [Fact]
    public void TakesAreNullWhenNotSet()
    {
        using var database = new SqliteDatabase();
        using (var context = database.CreateMigratedContentContext())
        {
            context.News!.Add(NewArticle());
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();
        var stored = reader.News!.Include("ArticleMeta").Single();

        Assert.Null(stored.EddieTake);
        Assert.Null(stored.DruTake);
        Assert.Empty(stored.Meta!);
    }

    [Fact]
    public void DuplicatePropertyOnOneArticleIsRejectedByTheDatabase()
    {
        using var database = new SqliteDatabase();
        var id = SaveArticle(database, Meta("description", "first"));

        using var context = database.CreateContentContext();
        context.ArticleMeta!.Add(new ArticleMeta
        {
            ArticleId = id, Property = "description", Value = "second"
        });

        Assert.Throws<DbUpdateException>(() => context.SaveChanges());
    }

    [Fact]
    public void SamePropertyOnTwoArticlesIsAllowed()
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateMigratedContentContext();
        var first = NewArticle("first");
        var second = NewArticle("second");
        Article.ApplyMeta(context, first, Meta("description", "one"));
        Article.ApplyMeta(context, second, Meta("description", "two"));
        context.News!.AddRange(first, second);
        context.SaveChanges();

        Assert.Equal(2, context.ArticleMeta!.Count());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)] // only the database's cascade can remove them
    public void DeletingAnArticleDeletesItsMeta(bool loadMeta)
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, Meta("description", "d", "keywords", "k"));

        using (var context = database.CreateContentContext())
        {
            var article = loadMeta
                ? context.News!.Include("ArticleMeta").Single()
                : context.News!.Single();
            context.News!.Remove(article);
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();
        Assert.Empty(reader.News!);
        Assert.Empty(reader.ArticleMeta!);
    }

    [Fact]
    public void ByPermalinkOrIdIncludesTheMeta()
    {
        using var database = new SqliteDatabase();
        // the tag table that ByPermalinkOrId joins belongs to ServerContext
        using (var server = CreateServerContext(database))
            server.Database.EnsureCreated();
        var id = SaveArticle(database, Meta("twitter:card", "summary"));

        using (var context = CreateRuntimeContentContext(database))
        {
            var byPermalink = Article.ByPermalinkOrId(context, "two-takes");
            Assert.NotNull(byPermalink);
            Assert.Equal(Meta("twitter:card", "summary"), byPermalink.Meta);
        }

        using (var context = CreateRuntimeContentContext(database))
        {
            var byId = Article.ByPermalinkOrId(context, id.ToString());
            Assert.NotNull(byId);
            Assert.Equal(Meta("twitter:card", "summary"), byId.Meta);
        }
    }

    // ByPermalinkOrId takes the runtime ContentContext, which picks its
    // Sqlite mappings from the configured connection type
    private static readonly IConfiguration SqliteConfiguration =
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["API:Database:ConnectionType"] = "Sqlite",
                ["API:Database:ConnectionString"] = "DataSource=:memory:"
            })
            .Build();

    private static ContentContext CreateRuntimeContentContext(
        SqliteDatabase database) =>
        new(new DbContextOptionsBuilder<ContentContext>()
            .UseSqlite(database.Connection)
            .Options, SqliteConfiguration);

    private static ServerContext CreateServerContext(SqliteDatabase database) =>
        new(new DbContextOptionsBuilder<ServerContext>()
            .UseSqlite(database.Connection)
            .Options, SqliteConfiguration);

    // ---- ApplyMeta --------------------------------------------------------

    [Fact]
    public void ApplyMetaAddsUpdatesAndRemoves()
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, Meta("a", "1", "b", "2", "c", "3"));

        UpdateMeta(database, Meta("a", "1", "b", "20", "d", "4"));

        Assert.Equal(new Dictionary<string, string?>
            {
                ["a"] = "1", ["b"] = "20", ["d"] = "4"
            },
            StoredMeta(database));
    }

    [Fact]
    public void ApplyMetaWithAnEmptySetClearsTheMeta()
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, Meta("a", "1", "b", "2"));

        UpdateMeta(database, new Dictionary<string, string>());

        Assert.Empty(StoredMeta(database));
    }

    [Fact]
    public void ApplyMetaWithNullLeavesTheMetaAlone()
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, Meta("a", "1", "b", "2"));

        UpdateMeta(database, null);

        Assert.Equal(new Dictionary<string, string?> { ["a"] = "1", ["b"] = "2" },
            StoredMeta(database));
    }

    [Fact]
    public void ApplyMetaDropsDollarKeysTrimsKeysAndStoresNullAsEmpty()
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, new Dictionary<string, string>
        {
            ["$id"] = "7",
            [" $ref "] = "1",
            ["  description  "] = "  kept as sent  ",
            ["robots"] = null!
        });

        Assert.Equal(new Dictionary<string, string?>
            {
                ["description"] = "  kept as sent  ",
                ["robots"] = ""
            },
            StoredMeta(database));
    }

    [Fact]
    public void ApplyMetaUpdatesARowWhoseKeyChangesCase()
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, Meta("description", "old"));
        long id;
        using (var reader = database.CreateContentContext())
            id = reader.ArticleMeta!.Single().Id;

        UpdateMeta(database, Meta("Description", "new"));

        using var after = database.CreateContentContext();
        var row = after.ArticleMeta!.Single();
        Assert.Equal(id, row.Id);
        Assert.Equal("Description", row.Property);
        Assert.Equal("new", row.Value);
    }

    [Fact]
    public void ApplyMetaLoadsTheRowsOfATrackedArticleReadWithoutThem()
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, Meta("a", "1", "b", "2"));

        using (var context = database.CreateContentContext())
        {
            var article = context.News!.Single();
            Article.ApplyMeta(context, article, Meta("a", "10"));
            context.SaveChanges();
        }

        Assert.Equal(new Dictionary<string, string?> { ["a"] = "10" },
            StoredMeta(database));
    }

    [Fact]
    public void ApplyMetaRefusesAnInvalidSetAndChangesNothing()
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, Meta("a", "1"));

        using (var context = database.CreateContentContext())
        {
            var article = context.News!.Include("ArticleMeta").Single();
            Assert.Throws<ArgumentException>(() => Article.ApplyMeta(context,
                article, Meta("b", "2", " ", "blank")));
            context.SaveChanges();
        }

        Assert.Equal(new Dictionary<string, string?> { ["a"] = "1" },
            StoredMeta(database));
    }

    [Fact]
    public void AfterApplyMetaMetaReflectsTheRowsNotTheSubmission()
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateMigratedContentContext();
        var article = NewArticle();
        article.Meta = new Dictionary<string, string>
        {
            ["$id"] = "2", [" description "] = "d", ["robots"] = null!
        };

        Article.ApplyMeta(context, article, article.SubmittedMeta);

        Assert.Null(article.SubmittedMeta);
        Assert.Equal(Meta("description", "d", "robots", ""), article.Meta);
    }

    // ---- the Meta projection and the JSON shape ---------------------------

    [Fact]
    public void MetaProjectsTheRowsOrderedByProperty()
    {
        var article = NewArticle();
        article.ArticleMeta.Add(new ArticleMeta { Property = "twitter:card", Value = "summary" });
        article.ArticleMeta.Add(new ArticleMeta { Property = "description", Value = null });
        article.ArticleMeta.Add(new ArticleMeta { Property = "og:title", Value = "t" });

        var meta = article.Meta!;

        Assert.Equal(new[] { "description", "og:title", "twitter:card" },
            meta.Keys.ToArray());
        Assert.Equal("", meta["description"]);
        Assert.Null(article.SubmittedMeta);
    }

    [Fact]
    public void MetaIsAnEmptyObjectWithoutRows()
    {
        var article = NewArticle();

        Assert.NotNull(article.Meta);
        Assert.Empty(article.Meta);
        Assert.Null(article.SubmittedMeta);
    }

    [Theory]
    [InlineData("""{"title":"t","summary":"s","body":"b"}""")]
    [InlineData("""{"title":"t","summary":"s","body":"b","meta":null}""")]
    public void ABodyWithoutMetaSubmitsNone(string json)
    {
        var model = JsonSerializer.Deserialize<Article>(json, Web)!;

        Assert.Null(model.SubmittedMeta);
        Assert.Null(Article.ValidateTakesAndMeta(model));
    }

    [Fact]
    public void ABodyWithAnEmptyMetaObjectSubmitsAnEmptySet()
    {
        var model = JsonSerializer.Deserialize<Article>(
            """{"title":"t","summary":"s","body":"b","meta":{}}""", Web)!;

        Assert.NotNull(model.SubmittedMeta);
        Assert.Empty(model.SubmittedMeta);
    }

    [Fact]
    public void ABodyCarriesTakesAndMeta()
    {
        var model = JsonSerializer.Deserialize<Article>(
            """
            {"title":"t","summary":"s","body":"b",
             "eddieTake":"  Ride it.  ","druTake":"   ",
             "meta":{"description":"d","og:title":"o"}}
            """, Web)!;

        Assert.Equal("Ride it.", Article.NormalizeTake(model.EddieTake));
        Assert.Null(Article.NormalizeTake(model.DruTake));
        Assert.Equal(Meta("description", "d", "og:title", "o"),
            model.SubmittedMeta);
    }

    [Fact]
    public void SerialisesTakesAndMetaButNotTheRows()
    {
        var article = NewArticle();
        article.EddieTake = "e";
        article.ArticleMeta.Add(new ArticleMeta { Property = "description", Value = "d" });

        using var document = JsonDocument.Parse(
            JsonSerializer.Serialize(article, Web));
        var root = document.RootElement;

        Assert.Equal("e", root.GetProperty("eddieTake").GetString());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("druTake").ValueKind);
        Assert.Equal("d", root.GetProperty("meta").GetProperty("description").GetString());
        Assert.False(root.TryGetProperty("articleMeta", out _));
        Assert.False(root.TryGetProperty("submittedMeta", out _));
    }

    [Fact]
    public void APreservedResponseSentBackLeavesTheMetaUnchanged()
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, Meta("description", "d", "keywords", "k"));

        string json;
        using (var reader = database.CreateContentContext())
            json = JsonSerializer.Serialize(
                reader.News!.Include("ArticleMeta").Single(), Preserve);
        using (var document = JsonDocument.Parse(json))
            Assert.True(document.RootElement.GetProperty("meta")
                .TryGetProperty("$id", out _));

        // read back as the host reads a body, with Preserve
        var model = JsonSerializer.Deserialize<Article>(json, Preserve)!;
        Assert.Null(Article.ValidateTakesAndMeta(model));
        UpdateMeta(database, model.SubmittedMeta);

        Assert.Equal(new Dictionary<string, string?>
            {
                ["description"] = "d", ["keywords"] = "k"
            },
            StoredMeta(database));
    }

    [Fact]
    public void AnEchoedDollarIdIsAnOrdinaryKeyWithoutPreserveAndIsDropped()
    {
        using var database = new SqliteDatabase();
        SaveArticle(database, Meta("description", "d"));

        var model = JsonSerializer.Deserialize<Article>(
            """
            {"title":"t","summary":"s","body":"b",
             "meta":{"$id":"4","description":"d","keywords":"k"}}
            """, Web)!;
        Assert.True(model.SubmittedMeta!.ContainsKey("$id"));
        Assert.Null(Article.ValidateTakesAndMeta(model));
        UpdateMeta(database, model.SubmittedMeta);

        Assert.Equal(new Dictionary<string, string?>
            {
                ["description"] = "d", ["keywords"] = "k"
            },
            StoredMeta(database));
    }

    // ---- takes ------------------------------------------------------------

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData(" \t\r\n ", null)]
    [InlineData("Ride it.", "Ride it.")]
    [InlineData("  Ride it.  ", "Ride it.")]
    [InlineData("Ride it.\n", "Ride it.")]
    public void NormalizeTakeTrimsAndBlanksToNull(string? take, string? expected) =>
        Assert.Equal(expected, Article.NormalizeTake(take));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Ride it.")]
    [InlineData("Ride it.\r\n")]
    public void ValidateTakeAcceptsOneLine(string? take) =>
        Assert.Null(Article.ValidateTake("Eddie's take", take));

    [Theory]
    [InlineData("Ride it.\nThen fix it.")]
    [InlineData("Ride it.\rThen fix it.")]
    [InlineData("Ride it.\r\nThen fix it.")]
    public void ValidateTakeRejectsALineBreak(string take) =>
        Assert.Equal("Eddie's take must be a single line",
            Article.ValidateTake("Eddie's take", take));

    [Fact]
    public void ValidateTakeCountsTheLengthAfterTrimming()
    {
        var longest = new string('t', Article.TakeMaxLength);
        Assert.Null(Article.ValidateTake("Dru's take", longest));
        Assert.Null(Article.ValidateTake("Dru's take", "  " + longest + "  "));
        Assert.Equal("Dru's take must be at most 280 characters",
            Article.ValidateTake("Dru's take", longest + "t"));
    }

    [Fact]
    public void ValidateTakesAndMetaNamesTheFirstProblem()
    {
        var model = NewArticle();
        Assert.Null(Article.ValidateTakesAndMeta(model));

        model.Meta = Meta("", "x");
        Assert.Equal("Meta keys must not be blank",
            Article.ValidateTakesAndMeta(model));

        model.DruTake = "a\nb";
        Assert.Equal("Dru's take must be a single line",
            Article.ValidateTakesAndMeta(model));

        model.EddieTake = new string('e', Article.TakeMaxLength + 1);
        Assert.Equal("Eddie's take must be at most 280 characters",
            Article.ValidateTakesAndMeta(model));
    }

    // ---- meta rules -------------------------------------------------------

    [Fact]
    public void ValidateMetaAcceptsNullAndEmpty()
    {
        Assert.Null(Article.ValidateMeta(null));
        Assert.Null(Article.ValidateMeta(new Dictionary<string, string>()));
    }

    [Fact]
    public void ValidateMetaAcceptsTheMaximumLengths() =>
        Assert.Null(Article.ValidateMeta(Meta(
            new string('k', ArticleMeta.PropertyMaxLength),
            new string('v', ArticleMeta.ValueMaxLength))));

    [Fact]
    public void ValidateMetaAcceptsANullValue() =>
        Assert.Null(Article.ValidateMeta(
            new Dictionary<string, string> { ["robots"] = null! }));

    [Fact]
    public void ValidateMetaSkipsDollarKeys() =>
        Assert.Null(Article.ValidateMeta(Meta(
            "$id", new string('v', ArticleMeta.ValueMaxLength + 1),
            " $values", "",
            "description", "d")));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateMetaRejectsABlankKey(string key) =>
        Assert.Equal("Meta keys must not be blank",
            Article.ValidateMeta(Meta(key, "v")));

    [Fact]
    public void ValidateMetaRejectsAnOverlongKey() =>
        Assert.Equal("Meta keys must be at most 255 characters",
            Article.ValidateMeta(Meta(
                new string('k', ArticleMeta.PropertyMaxLength + 1), "v")));

    [Fact]
    public void ValidateMetaRejectsAnOverlongValue() =>
        Assert.Equal("Meta value for 'description' must be at most 2048 characters",
            Article.ValidateMeta(Meta(
                "description", new string('v', ArticleMeta.ValueMaxLength + 1))));

    [Theory]
    [InlineData("description", " description ")]
    [InlineData("description", "Description")]
    public void ValidateMetaRejectsKeysThatMatchOnceTrimmedOrIgnoringCase(
        string first, string second) =>
        Assert.NotNull(Article.ValidateMeta(Meta(first, "1", second, "2")));
}

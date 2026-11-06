using System.Text.Json;
using Druware.Server.Content.Entities;

namespace Druware.Server.Content.UnitTests;

public class ProductDisableRedirectTests
{
    // a migrated database holding the products "old" and "new"; returns
    // the id of "new"
    private static long Seed(SqliteDatabase database)
    {
        database.CreateMigratedContentContext().Dispose();
        using var context = database.CreateRuntimeContentContext();
        context.Products!.Add(new Product { Name = "Old", Short = "old" });
        var target = new Product { Name = "New", Short = "new" };
        context.Products!.Add(target);
        context.SaveChanges();
        return (long)target.ProductId!;
    }

    // ---- persistence ------------------------------------------------------

    [Fact]
    public void IsDisabledAndRedirectToRoundTrip()
    {
        using var database = new SqliteDatabase();
        Seed(database);
        using (var context = database.CreateRuntimeContentContext())
        {
            var product = Product.ByShortOrId(context, "old")!;
            product.IsDisabled = true;
            product.RedirectTo = "new";
            context.SaveChanges();
        }

        using var reader = database.CreateRuntimeContentContext();
        var stored = Product.ByShortOrId(reader, "old")!;
        Assert.True(stored.IsDisabled);
        Assert.Equal("new", stored.RedirectTo);
    }

    [Fact]
    public void ANewProductIsEnabledWithNoRedirect()
    {
        using var database = new SqliteDatabase();
        Seed(database);

        using var reader = database.CreateRuntimeContentContext();
        var stored = Product.ByShortOrId(reader, "new")!;
        Assert.False(stored.IsDisabled);
        Assert.Null(stored.RedirectTo);
    }

    // ---- NormalizeRedirectTo ----------------------------------------------

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData(" new ", "new")]
    [InlineData("https://example.com/x", "https://example.com/x")]
    public void NormalizeRedirectToTrimsAndBlanksToNull(string? value,
        string? expected) =>
        Assert.Equal(expected, Product.NormalizeRedirectTo(value));

    // ---- ValidateRedirectTo -----------------------------------------------

    private static string? Validate(SqliteDatabase database, string? redirectTo,
        string shortName = "added", long? productId = null)
    {
        using var context = database.CreateRuntimeContentContext();
        return Product.ValidateRedirectTo(context,
            new Product { Short = shortName, RedirectTo = redirectTo },
            productId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("new")]
    [InlineData("http://example.com")]
    [InlineData("https://example.com/products/new?x=1")]
    public void ValidateAcceptsNoRedirectAProductShortOrAWebAddress(string? redirectTo)
    {
        using var database = new SqliteDatabase();
        Seed(database);
        Assert.Null(Validate(database, redirectTo));
    }

    [Fact]
    public void ValidateAcceptsAProductId()
    {
        using var database = new SqliteDatabase();
        var id = Seed(database);
        Assert.Null(Validate(database, id.ToString()));
    }

    [Theory]
    [InlineData("missing")]
    [InlineData("999")]
    [InlineData("/products/new")]
    [InlineData("ftp://example.com/new")]
    [InlineData("mailto:dru@example.com")]
    [InlineData("example.com/new")]
    public void ValidateRejectsAnythingElse(string redirectTo)
    {
        using var database = new SqliteDatabase();
        Seed(database);
        Assert.Equal(
            "Redirect must be an existing product's short or id, or a full http or https address",
            Validate(database, redirectTo));
    }

    [Fact]
    public void ValidateRejectsARedirectToTheProductsOwnShort()
    {
        using var database = new SqliteDatabase();
        Seed(database);
        Assert.Equal("A product cannot redirect to itself",
            Validate(database, "added", shortName: "added"));
    }

    [Fact]
    public void ValidateRejectsARedirectToTheStoredProductOnUpdate()
    {
        using var database = new SqliteDatabase();
        var id = Seed(database);

        // by id, and by its stored short while the update renames it
        Assert.Equal("A product cannot redirect to itself",
            Validate(database, id.ToString(), shortName: "new", productId: id));
        Assert.Equal("A product cannot redirect to itself",
            Validate(database, "new", shortName: "renamed", productId: id));
    }

    [Fact]
    public void ValidateEnforcesTheMaximumLength()
    {
        using var database = new SqliteDatabase();
        Seed(database);
        var prefix = "https://example.com/";
        var longest = prefix + new string('a', Product.RedirectToMaxLength - prefix.Length);

        Assert.Null(Validate(database, longest));
        Assert.Equal($"Redirect must be at most {Product.RedirectToMaxLength} characters",
            Validate(database, longest + "a"));
    }

    // ---- ResolveRedirect ---------------------------------------------------

    [Fact]
    public void ResolveRedirectGivesTheTargetsCurrentShort()
    {
        using var database = new SqliteDatabase();
        var id = Seed(database);
        using var context = database.CreateRuntimeContentContext();

        var byShort = new Product { RedirectTo = "new" };
        Product.ResolveRedirect(context, byShort);
        Assert.Equal("new", byShort.RedirectShort);

        // an id keeps working after the target is renamed
        Product.ByShortOrId(context, "new")!.Short = "renamed";
        context.SaveChanges();
        var byId = new Product { RedirectTo = id.ToString() };
        Product.ResolveRedirect(context, byId);
        Assert.Equal("renamed", byId.RedirectShort);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("https://example.com/new")]
    [InlineData("deleted")]
    public void ResolveRedirectLeavesRedirectShortNullOtherwise(string? redirectTo)
    {
        using var database = new SqliteDatabase();
        Seed(database);
        using var context = database.CreateRuntimeContentContext();

        var product = new Product { RedirectTo = redirectTo, RedirectShort = "stale" };
        Product.ResolveRedirect(context, product);
        Assert.Null(product.RedirectShort);
    }

    // ---- JSON --------------------------------------------------------------

    [Fact]
    public void SerialisesTheNewFieldsAndReadsThemFromABody()
    {
        var json = JsonSerializer.Serialize(
            new Product
            {
                Short = "old", IsDisabled = true, RedirectTo = "7", RedirectShort = "new"
            },
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        Assert.True(root.GetProperty("isDisabled").GetBoolean());
        Assert.Equal("7", root.GetProperty("redirectTo").GetString());
        Assert.Equal("new", root.GetProperty("redirectShort").GetString());

        var body = JsonSerializer.Deserialize<Product>(
            """{"short":"old","isDisabled":true,"redirectTo":"new"}""",
            new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        Assert.True(body.IsDisabled);
        Assert.Equal("new", body.RedirectTo);
    }
}

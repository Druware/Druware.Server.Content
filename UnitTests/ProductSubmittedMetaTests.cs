using System.Text.Json;
using System.Text.Json.Serialization;
using Druware.Server.Content.Entities;
using Microsoft.EntityFrameworkCore;

namespace Druware.Server.Content.UnitTests;

public class ProductSubmittedMetaTests
{
    private static readonly JsonSerializerOptions Web =
        new(JsonSerializerDefaults.Web);

    private static readonly JsonSerializerOptions Preserve =
        new(JsonSerializerDefaults.Web)
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

    private static Dictionary<string, string> Meta(params string[] pairs)
    {
        Dictionary<string, string> meta = new();
        for (var i = 0; i < pairs.Length; i += 2)
            meta[pairs[i]] = pairs[i + 1];
        return meta;
    }

    private static Product Body(string json) =>
        JsonSerializer.Deserialize<Product>(json, Web)!;

    // saves a product carrying the given meta rows
    private static void SaveProduct(SqliteDatabase database,
        Dictionary<string, string> meta)
    {
        using var context = database.CreateMigratedContentContext();
        var product = new Product { Name = "Bike", Short = "bike" };
        foreach (var (property, value) in meta)
            product.ProductMeta.Add(new ProductMeta { Property = property, Value = value });
        context.Products!.Add(product);
        context.SaveChanges();
    }

    // the meta rows as stored, read through a fresh context
    private static Dictionary<string, string?> StoredMeta(SqliteDatabase database)
    {
        using var reader = database.CreateContentContext();
        return reader.ProductMeta!
            .ToDictionary(m => m.Property!, m => m.Value);
    }

    // the meta step of ProductController.Update, given the dictionary the
    // controller reads from the body
    private static void UpdateMeta(SqliteDatabase database,
        Dictionary<string, string>? meta)
    {
        using var context = database.CreateContentContext();
        var obj = context.Products!.Single();

        if (meta != null)
        {
            context.Entry(obj).Collection(p => p.ProductMeta).Load();

            foreach (var m in meta)
            {
                if (string.IsNullOrWhiteSpace(m.Key)) continue;

                var pm = obj.ProductMeta
                    .FirstOrDefault(e => e.Property == m.Key);
                if (pm == null)
                {
                    pm = new ProductMeta();
                    pm.ProductId = (long)obj.ProductId!;
                    obj.ProductMeta.Add(pm);
                }

                pm.Property = m.Key;
                pm.Value = m.Value;
            }

            var removed = obj.ProductMeta
                .Where(e => e.Property == null || !meta.ContainsKey(e.Property))
                .ToList();
            foreach (var pm in removed)
            {
                context.ProductMeta?.Remove(pm);
                obj.ProductMeta.Remove(pm);
            }
        }

        context.SaveChanges();
    }

    // ---- what a body submits ----------------------------------------------

    [Theory]
    [InlineData("""{"name":"Bike","short":"bike"}""")]
    [InlineData("""{"name":"Bike","short":"bike","meta":null}""")]
    public void ABodyWithoutMetaSubmitsNone(string json) =>
        Assert.Null(Body(json).SubmittedMeta);

    [Fact]
    public void ABodyWithAnEmptyMetaObjectSubmitsAnEmptySet()
    {
        var model = Body("""{"name":"Bike","short":"bike","meta":{}}""");

        Assert.NotNull(model.SubmittedMeta);
        Assert.Empty(model.SubmittedMeta);
    }

    [Fact]
    public void ABodyCarriesMeta() =>
        Assert.Equal(Meta("description", "d", "og:title", "o"),
            Body("""
                 {"name":"Bike","short":"bike",
                  "meta":{"description":"d","og:title":"o"}}
                 """).SubmittedMeta);

    // the host reads bodies with ReferenceHandler.Preserve
    [Fact]
    public void APreservedResponseSentBackSubmitsItsMeta()
    {
        var product = new Product { ProductId = 7, Name = "Bike", Short = "bike" };
        product.ProductMeta.Add(new ProductMeta { Property = "description", Value = "d" });
        var json = JsonSerializer.Serialize(product, Preserve);

        var model = JsonSerializer.Deserialize<Product>(json, Preserve)!;

        Assert.Equal(Meta("description", "d"), model.SubmittedMeta);
        Assert.Null(JsonSerializer.Deserialize<Product>(
            """{"name":"Bike","short":"bike"}""", Preserve)!.SubmittedMeta);
    }

    // model validation and serialisation read Meta, which caches its
    // projection; that must not turn into a submission
    [Theory]
    [InlineData("""{"name":"Bike","short":"bike"}""")]
    [InlineData("""{"name":"Bike","short":"bike","meta":null}""")]
    public void ReadingMetaDoesNotSubmitIt(string json)
    {
        var model = Body(json);

        Assert.NotNull(model.Meta);
        Assert.Empty(model.Meta);
        JsonSerializer.Serialize(model, Web);

        Assert.Null(model.SubmittedMeta);
    }

    // ---- the Meta projection and the JSON shape ---------------------------

    [Fact]
    public void MetaProjectsTheRowsOfALoadedProduct()
    {
        using var database = new SqliteDatabase();
        SaveProduct(database, Meta("description", "d", "og:title", "o"));

        using var reader = database.CreateContentContext();
        var stored = reader.Products!.Include("ProductMeta").Single();

        Assert.Equal(Meta("description", "d", "og:title", "o"), stored.Meta);
        Assert.Null(stored.SubmittedMeta);
    }

    [Fact]
    public void SerialisesAsBefore()
    {
        var product = new Product { ProductId = 7, Name = "Bike", Short = "bike" };
        product.ProductMeta.Add(new ProductMeta { Property = "description", Value = null });

        Assert.Equal(
            """
            {"productId":7,"name":"Bike","short":"bike","summary":null,
            "description":null,"created":null,"updated":null,"license":null,
            "downloadUrl":null,"documentationUrl":null,"iconUrl":null,
            "appStoreApple":null,"appStoreMs":null,"appStoreGoogle":null,
            "appStoreAmazon":null,"directOsx":null,"directWinArm":null,
            "directWinX64":null,"directGnome":null,"directKde":null,
            "directHaiku":null,"repoGithub":null,"tags":[],
            "meta":{"description":""},"collections":[]}
            """.ReplaceLineEndings(""),
            JsonSerializer.Serialize(product, Web));
        Assert.Null(product.SubmittedMeta);
    }

    // ---- the controller's update ------------------------------------------

    [Theory]
    [InlineData("""{"name":"Bike","short":"bike"}""")]
    [InlineData("""{"name":"Bike","short":"bike","meta":null}""")]
    public void APutWithoutMetaLeavesTheRowsAlone(string json)
    {
        using var database = new SqliteDatabase();
        SaveProduct(database, Meta("description", "d", "og:title", "o"));

        var model = Body(json);
        _ = model.Meta; // as model validation does
        UpdateMeta(database, model.SubmittedMeta);

        Assert.Equal(new Dictionary<string, string?>
            {
                ["description"] = "d", ["og:title"] = "o"
            },
            StoredMeta(database));
    }

    [Fact]
    public void APutWithAnEmptyMetaObjectClearsTheRows()
    {
        using var database = new SqliteDatabase();
        SaveProduct(database, Meta("description", "d", "og:title", "o"));

        var model = Body("""{"name":"Bike","short":"bike","meta":{}}""");
        UpdateMeta(database, model.SubmittedMeta);

        Assert.Empty(StoredMeta(database));
    }

    [Fact]
    public void APutWithMetaReplacesTheSet()
    {
        using var database = new SqliteDatabase();
        SaveProduct(database, Meta("a", "1", "b", "2", "c", "3"));

        var model = Body("""
                         {"name":"Bike","short":"bike",
                          "meta":{"a":"1","b":"20","d":"4"}}
                         """);
        UpdateMeta(database, model.SubmittedMeta);

        Assert.Equal(new Dictionary<string, string?>
            {
                ["a"] = "1", ["b"] = "20", ["d"] = "4"
            },
            StoredMeta(database));
    }
}

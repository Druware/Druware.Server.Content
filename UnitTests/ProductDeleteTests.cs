using Druware.Server.Content.Entities;
using Druware.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Druware.Server.Content.UnitTests;

// ProductController.Remove looks the product up with Product.ByShortOrId,
// removes it and saves, so these do the same on a fresh runtime
// ContentContext, as the host builds it, where only what ByShortOrId
// includes is tracked and the database has to do the rest.
public class ProductDeleteTests
{
    // a product with a row in each table named in children, or in every
    // table that refers to a product when children is "all"
    private static long SeedProductWithChildren(SqliteDatabase database,
        string children = "all")
    {
        bool With(string child) => children == "all" || children == child;

        database.CreateMigratedContentContext().Dispose();
        using var context = database.CreateRuntimeContentContext();
        var product = new Product { Name = "Widget", Short = "widget" };
        if (With("tag"))
            product.ProductTags.Add(new ProductTag { Tag = new Tag { Name = "tools" } });
        if (With("meta"))
            product.ProductMeta.Add(new ProductMeta { Property = "og:title", Value = "Widget" });
        context.Collections!.Add(new Collection { Name = "Utilities" });
        if (With("collection"))
            product.CollectionProducts.Add(new CollectionProduct
            {
                Collection = context.Collections.Local.Single()
            });
        context.Products!.Add(product);
        context.SaveChanges();

        if (With("release"))
            context.ProductReleases!.Add(new ProductRelease
            {
                ProductId = product.ProductId,
                Title = "1.0",
                Body = "First release"
            });
        context.SaveChanges();
        return (long)product.ProductId!;
    }

    // by id, ByShortOrId includes the tags; by short it includes nothing
    [Theory]
    [InlineData("all", true)]
    [InlineData("all", false)]
    [InlineData("tag", true)]
    [InlineData("tag", false)]
    [InlineData("meta", true)]
    [InlineData("meta", false)]
    [InlineData("collection", true)]
    [InlineData("collection", false)]
    [InlineData("release", true)]
    [InlineData("release", false)]
    public void DeletingAProductFoundByShortOrIdDeletesItsChildRows(
        string children, bool byId)
    {
        using var database = new SqliteDatabase();
        var id = SeedProductWithChildren(database, children);

        using (var context = database.CreateRuntimeContentContext())
        {
            var product = Product.ByShortOrId(context,
                byId ? id.ToString() : "widget");
            Assert.NotNull(product);
            context.Products!.Remove(product);
            context.SaveChanges();
        }

        using var reader = database.CreateRuntimeContentContext();
        Assert.Empty(reader.Products!);
        Assert.Empty(reader.ProductTags!);
        Assert.Empty(reader.ProductMeta!);
        Assert.Empty(reader.CollectionProducts!);
        Assert.Empty(reader.ProductReleases!);
        // the shared tag and the collection itself are kept
        Assert.Single(reader.Collections!);
    }

    // ---- PostgreSQL --------------------------------------------------------

    // The host runs on PostgreSQL, so the same delete is run there when
    // CONTENT_TEST_POSTGRES holds a connection string to a throwaway server;
    // the test creates and drops its own database on it. The PostgreSql
    // migrations do not apply to an empty database (article-v2 alters an
    // article table no migration creates), so the schema comes from the
    // runtime model, which the snapshot test ties to the migrations.
    [PostgreSqlTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void DeletingAProductFoundByShortOrIdDeletesItsChildRowsOnPostgreSql(
        bool byId)
    {
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(
            Environment.GetEnvironmentVariable(PostgreSqlTheoryAttribute.Variable))
        {
            Database = $"content_test_{Guid.NewGuid():N}"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["API:Database:ConnectionType"] = "PostgreSql",
                ["API:Database:ConnectionString"] = builder.ConnectionString
            })
            .Build();
        ContentContext Create() =>
            new(new DbContextOptionsBuilder<ContentContext>()
                .UseNpgsql(builder.ConnectionString).Options, configuration);

        // EnsureCreated cannot be used: the PostgreSQL ProductRelease mapping
        // has the column type "text)" for body (the migrations say "text"),
        // so the generated script is corrected before it is run
        using (var admin = new Npgsql.NpgsqlConnection(
                   new Npgsql.NpgsqlConnectionStringBuilder(builder.ConnectionString)
                       { Database = "postgres" }.ConnectionString))
        {
            admin.Open();
            using var command = admin.CreateCommand();
            command.CommandText = $"CREATE DATABASE {builder.Database}";
            command.ExecuteNonQuery();
        }
        try
        {
            using (var context = Create())
                context.Database.ExecuteSqlRaw(context.Database
                    .GenerateCreateScript().Replace("text)", "text"));

            long id;
            using (var context = Create())
            {
                var product = new Product { Name = "Widget", Short = "widget" };
                product.ProductTags.Add(new ProductTag { Tag = new Tag { Name = "tools" } });
                product.ProductMeta.Add(new ProductMeta { Property = "og:title", Value = "Widget" });
                product.CollectionProducts.Add(new CollectionProduct
                {
                    Collection = new Collection { Name = "Utilities" }
                });
                context.Products!.Add(product);
                context.SaveChanges();
                context.ProductReleases!.Add(new ProductRelease
                {
                    ProductId = product.ProductId,
                    Title = "1.0",
                    Body = "First release"
                });
                context.SaveChanges();
                id = (long)product.ProductId!;
            }

            using (var context = Create())
            {
                var product = Product.ByShortOrId(context,
                    byId ? id.ToString() : "widget");
                Assert.NotNull(product);
                context.Products!.Remove(product);
                context.SaveChanges();
            }

            using var reader = Create();
            Assert.Empty(reader.Products!);
            Assert.Empty(reader.ProductTags!);
            Assert.Empty(reader.ProductMeta!);
            Assert.Empty(reader.CollectionProducts!);
            Assert.Empty(reader.ProductReleases!);
            Assert.Single(reader.Collections!);
        }
        finally
        {
            using var context = Create();
            context.Database.EnsureDeleted();
        }
    }
}

/// <summary>
/// A theory that runs only when CONTENT_TEST_POSTGRES holds a connection
/// string to a PostgreSQL server, and is skipped otherwise.
/// </summary>
public sealed class PostgreSqlTheoryAttribute : TheoryAttribute
{
    public const string Variable = "CONTENT_TEST_POSTGRES";

    public PostgreSqlTheoryAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(Variable)))
            Skip = $"Set {Variable} to a PostgreSQL connection string to run";
    }
}

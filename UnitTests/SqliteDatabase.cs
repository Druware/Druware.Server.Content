using Druware.Server;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Druware.Server.Content.UnitTests;

/// <summary>
/// An in-memory Sqlite database that lives as long as this object: the
/// connection is held open, because closing it discards the database.
/// </summary>
public sealed class SqliteDatabase : IDisposable
{
    public SqliteConnection Connection { get; } = new("DataSource=:memory:");

    public SqliteDatabase() => Connection.Open();

    private static readonly IConfiguration SqliteConfiguration =
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["API:Database:ConnectionType"] = "Sqlite",
                ["API:Database:ConnectionString"] = "DataSource=:memory:"
            })
            .Build();

    public ContentContextSqlite CreateContentContext() =>
        new(new DbContextOptionsBuilder<ContentContextSqlite>()
            .UseSqlite(Connection)
            .Options, null);

    /// <summary>
    /// Creates the Druware.Server tables, the shared tag pool among them,
    /// as a host does by migrating its ServerContext before the Content
    /// one. The Content tag joins refer to the tag table, and SQLite
    /// refuses to write to a table whose foreign key names a missing table,
    /// which the ProductDisableRedirect rebuild of product_tag does.
    /// EnsureCreated does nothing once the database has a table.
    /// </summary>
    public void CreateServerSchema()
    {
        using var server = new ServerContext(
            new DbContextOptionsBuilder<ServerContext>()
                .UseSqlite(Connection)
                .Options, SqliteConfiguration);
        server.Database.EnsureCreated();
    }

    /// <summary>
    /// Returns a context on a database with the Server tables and every
    /// Content migration applied.
    /// </summary>
    public ContentContextSqlite CreateMigratedContentContext()
    {
        CreateServerSchema();
        var context = CreateContentContext();
        context.Database.Migrate();
        return context;
    }

    /// <summary>
    /// Returns the runtime ContentContext, as the host builds it for Sqlite,
    /// on this database; the controllers and the Product helpers take it.
    /// </summary>
    public ContentContext CreateRuntimeContentContext() =>
        new(new DbContextOptionsBuilder<ContentContext>()
            .UseSqlite(Connection)
            .Options, SqliteConfiguration);

    public void Dispose() => Connection.Dispose();
}

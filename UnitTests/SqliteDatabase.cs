using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Druware.Server.Content.UnitTests;

/// <summary>
/// An in-memory Sqlite database that lives as long as this object: the
/// connection is held open, because closing it discards the database.
/// </summary>
public sealed class SqliteDatabase : IDisposable
{
    public SqliteConnection Connection { get; } = new("DataSource=:memory:");

    public SqliteDatabase() => Connection.Open();

    public ContentContextSqlite CreateContentContext() =>
        new(new DbContextOptionsBuilder<ContentContextSqlite>()
            .UseSqlite(Connection)
            .Options, null);

    /// <summary>
    /// Returns a context on a database with every Content migration applied.
    /// </summary>
    public ContentContextSqlite CreateMigratedContentContext()
    {
        var context = CreateContentContext();
        context.Database.Migrate();
        return context;
    }

    public void Dispose() => Connection.Dispose();
}

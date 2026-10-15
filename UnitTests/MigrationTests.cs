using Microsoft.EntityFrameworkCore;

namespace Druware.Server.Content.UnitTests;

public class MigrationTests
{
    [Fact]
    public void AllSqliteMigrationsApplyToAnEmptyDatabase()
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateContentContext();

        var all = context.Database.GetMigrations().ToList();
        Assert.NotEmpty(all);

        context.Database.Migrate();

        Assert.Equal(all, context.Database.GetAppliedMigrations());
        Assert.Empty(context.Database.GetPendingMigrations());
        Assert.Contains(all, m => m.EndsWith("_SiteSettings"));
        Assert.Contains(all, m => m.EndsWith("_ArticleTakesAndMeta"));
        Assert.Contains(all, m => m.EndsWith("_Reviews"));
    }

    [Fact]
    public void SqliteModelHasNoChangesMissingFromTheMigrations()
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateContentContext();

        Assert.False(context.Database.HasPendingModelChanges());
    }

    // building a model needs no server, so the SQL Server and PostgreSQL
    // snapshots can be checked against their models here too
    [Fact]
    public void MicrosoftModelHasNoChangesMissingFromTheMigrations()
    {
        using var context = new ContentContextMicrosoft(
            new DbContextOptionsBuilder<ContentContextMicrosoft>()
                .UseSqlServer("Server=unused").Options, null);

        Assert.False(context.Database.HasPendingModelChanges());
    }

    [Fact]
    public void PostgreSqlModelHasNoChangesMissingFromTheMigrations()
    {
        using var context = new ContentContextPostgreSql(
            new DbContextOptionsBuilder<ContentContextPostgreSql>()
                .UseNpgsql("Host=localhost;Database=unused").Options, null);

        Assert.False(context.Database.HasPendingModelChanges());
    }
}

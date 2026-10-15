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
    }

    [Fact]
    public void SqliteModelHasNoChangesMissingFromTheMigrations()
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateContentContext();

        Assert.False(context.Database.HasPendingModelChanges());
    }
}

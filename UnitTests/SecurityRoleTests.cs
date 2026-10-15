using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Druware.Server.Content.UnitTests;

public class SecurityRoleTests
{
    // ContentContext.ConfigureSecurityRoles takes the runtime ServerContext,
    // which picks its Sqlite mappings from the configured connection type.
    private static readonly IConfiguration SqliteConfiguration =
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["API:Database:ConnectionType"] = "Sqlite",
                ["API:Database:ConnectionString"] = "DataSource=:memory:"
            })
            .Build();

    private static ServerContext CreateServerContext(SqliteDatabase database) =>
        new(new DbContextOptionsBuilder<ServerContext>()
            .UseSqlite(database.Connection)
            .Options, SqliteConfiguration);

    [Fact]
    public void ConfigureSecurityRolesIsIdempotent()
    {
        using var database = new SqliteDatabase();
        using (var context = CreateServerContext(database))
        {
            context.Database.EnsureCreated();
            ContentContext.ConfigureSecurityRoles(context);
        }

        using (var context = CreateServerContext(database))
            ContentContext.ConfigureSecurityRoles(context);

        using var reader = CreateServerContext(database);
        Assert.Equal(1, reader.Roles.Count(r =>
            r.Name == SiteContentSecurityRole.Author));
        Assert.Equal(1, reader.Roles.Count(r =>
            r.Name == SiteContentSecurityRole.Editor));
    }

    [Fact]
    public void SiteContentRolesAreSeededWithTheirDescriptions()
    {
        using var database = new SqliteDatabase();
        using var context = CreateServerContext(database);
        context.Database.EnsureCreated();
        ContentContext.ConfigureSecurityRoles(context);

        var roles = context.Roles.OfType<Druware.Server.Entities.Role>()
            .Where(r => r.Name == SiteContentSecurityRole.Author ||
                        r.Name == SiteContentSecurityRole.Editor)
            .ToDictionary(r => r.Name!, r => r.Description);

        Assert.Equal("Site Content Author", roles[SiteContentSecurityRole.Author]);
        Assert.Equal("Site Content Editor", roles[SiteContentSecurityRole.Editor]);
    }
}

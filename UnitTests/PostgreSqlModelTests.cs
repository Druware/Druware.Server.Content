using Druware.Server.Content.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Druware.Server.Content.UnitTests;

// Building a model needs no server, so these run without PostgreSQL. Npgsql
// maps DateTime to timestamptz by default, which rejects local times, so
// both PostgreSQL model paths must store `updated` without a time zone.
public class PostgreSqlModelTests
{
    private const string NoServer = "Host=localhost;Database=unused";
    private const string WithoutTimeZone = "timestamp without time zone";

    private static string? UpdatedColumnType(DbContext context) =>
        context.Model.FindEntityType(typeof(SiteSetting))!
            .FindProperty(nameof(SiteSetting.Updated))!
            .GetColumnType();

    [Fact]
    public void MigrationContextStoresUpdatedWithoutTimeZone()
    {
        using var context = new ContentContextPostgreSql(
            new DbContextOptionsBuilder<ContentContextPostgreSql>()
                .UseNpgsql(NoServer).Options, null);

        Assert.Equal(WithoutTimeZone, UpdatedColumnType(context));
    }

    [Fact]
    public void RuntimeContextStoresUpdatedWithoutTimeZone()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["API:Database:ConnectionType"] = "PostgreSql",
                ["API:Database:ConnectionString"] = NoServer
            })
            .Build();
        using var context = new ContentContext(
            new DbContextOptionsBuilder<ContentContext>()
                .UseNpgsql(NoServer).Options, configuration);

        Assert.Equal(WithoutTimeZone, UpdatedColumnType(context));
    }

    // Reading the provider name runs OnConfiguring without connecting.
    [Fact]
    public void MigrationContextBuiltFromConfigurationUsesNpgsql()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["API:Database:ConnectionType"] = "PostgreSql",
                ["API:Database:ConnectionString"] = NoServer
            })
            .Build();
        using var context = new ContentContextPostgreSql(configuration);

        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL",
            context.Database.ProviderName);
    }
}

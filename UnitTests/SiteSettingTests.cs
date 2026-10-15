using System.Text.Json;
using Druware.Server.Content.Entities;
using Microsoft.EntityFrameworkCore;

namespace Druware.Server.Content.UnitTests;

public class SiteSettingTests
{
    private static readonly DateTime Stamp = new(2026, 10, 15, 9, 30, 0);

    private static SiteSetting NewSetting(string key, string? value = null) =>
        new()
        {
            Key = key,
            Value = value,
            Description = "Where " + key + " appears",
            Updated = Stamp
        };

    [Fact]
    public void InsertAndReadBack()
    {
        using var database = new SqliteDatabase();
        using (var context = database.CreateMigratedContentContext())
        {
            context.SiteSettings!.Add(NewSetting("social.youtube",
                "https://www.youtube.com/@example"));
            context.SaveChanges();
        }

        using var reader = database.CreateContentContext();
        var setting = reader.SiteSettings!.Single();

        Assert.NotNull(setting.SettingId);
        Assert.Equal("social.youtube", setting.Key);
        Assert.Equal("https://www.youtube.com/@example", setting.Value);
        Assert.Equal("Where social.youtube appears", setting.Description);
        Assert.Equal(Stamp, setting.Updated);
    }

    [Fact]
    public void DuplicateKeyIsRejectedByTheDatabase()
    {
        using var database = new SqliteDatabase();
        using (var context = database.CreateMigratedContentContext())
        {
            context.SiteSettings!.Add(NewSetting("social.strava"));
            context.SaveChanges();
        }

        using var second = database.CreateContentContext();
        second.SiteSettings!.Add(NewSetting("social.strava"));

        Assert.Throws<DbUpdateException>(() => second.SaveChanges());
    }

    [Fact]
    public void ByKeyOrIdFindsByIdAndByKey()
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateMigratedContentContext();
        context.SiteSettings!.Add(NewSetting("newsletter.list", "rambln"));
        var other = NewSetting("support.donationUrl");
        context.SiteSettings!.Add(other);
        context.SaveChanges();

        var byKey = SiteSetting.ByKeyOrId(context, "newsletter.list");
        Assert.NotNull(byKey);
        Assert.Equal("rambln", byKey.Value);

        var byId = SiteSetting.ByKeyOrId(context, other.SettingId!.Value.ToString());
        Assert.NotNull(byId);
        Assert.Equal("support.donationUrl", byId.Key);
    }

    [Theory]
    [InlineData("home.archivePermalink")]
    [InlineData("999")]
    [InlineData("")]
    public void ByKeyOrIdReturnsNullWhenNothingMatches(string value)
    {
        using var database = new SqliteDatabase();
        using var context = database.CreateMigratedContentContext();
        context.SiteSettings!.Add(NewSetting("social.youtube"));
        context.SaveChanges();

        Assert.Null(SiteSetting.ByKeyOrId(context, value));
    }

    [Theory]
    [InlineData("social.youtube")]
    [InlineData("social.strava")]
    [InlineData("support.donationUrl")]
    [InlineData("newsletter.list")]
    [InlineData("home.archivePermalink")]
    [InlineData("blpg.disclaimerDocument")]
    [InlineData("blpg.sideloadDocument")]
    [InlineData("a")]
    [InlineData("a1.2b.C3")]
    public void IsValidKeyAcceptsDottedKeys(string key) =>
        Assert.True(SiteSetting.IsValidKey(key));

    [Fact]
    public void IsValidKeyAcceptsTheMaximumLength() =>
        Assert.True(SiteSetting.IsValidKey(new string('a', SiteSetting.KeyMaxLength)));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1social")]
    [InlineData("123")]
    [InlineData(".social")]
    [InlineData("social.")]
    [InlineData("social..youtube")]
    [InlineData("social youtube")]
    [InlineData(" social")]
    [InlineData("social/youtube")]
    [InlineData("social\\youtube")]
    [InlineData("social-youtube")]
    [InlineData("social_youtube")]
    [InlineData("social.youtube\n")]
    public void IsValidKeyRejectsMalformedKeys(string? key) =>
        Assert.False(SiteSetting.IsValidKey(key));

    [Fact]
    public void IsValidKeyRejectsKeysOverTheMaximumLength() =>
        Assert.False(SiteSetting.IsValidKey(new string('a', SiteSetting.KeyMaxLength + 1)));

    [Fact]
    public void ValidateAcceptsAWellFormedSetting() =>
        Assert.Null(SiteSetting.Validate(NewSetting("social.youtube")));

    [Fact]
    public void ValidateAcceptsAMissingDescriptionAndValue() =>
        Assert.Null(SiteSetting.Validate(new SiteSetting { Key = "social.youtube" }));

    [Fact]
    public void ValidateRejectsABadKey() =>
        Assert.NotNull(SiteSetting.Validate(NewSetting("social..youtube")));

    [Fact]
    public void ValidateRejectsAnOverlongDescription()
    {
        var setting = NewSetting("social.youtube");
        setting.Description = new string('d', SiteSetting.DescriptionMaxLength);
        Assert.Null(SiteSetting.Validate(setting));

        setting.Description += "d";
        Assert.NotNull(SiteSetting.Validate(setting));
    }

    [Fact]
    public void SerialisesAsScalarsOnly()
    {
        var json = JsonSerializer.Serialize(NewSetting("social.youtube", "x"),
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        using var document = JsonDocument.Parse(json);
        var names = document.RootElement.EnumerateObject()
            .Select(p => p.Name).ToArray();

        Assert.Equal(
            new[] { "settingId", "key", "value", "description", "updated" },
            names);
    }
}

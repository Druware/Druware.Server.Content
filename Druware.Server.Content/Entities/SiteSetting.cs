using System.Text.RegularExpressions;

namespace Druware.Server.Content.Entities;

public partial class SiteSetting
{
    public const int KeyMaxLength = 128;
    public const int DescriptionMaxLength = 512;

    public long? SettingId { get; set; } = null;
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; } = null;
    public string? Description { get; set; } = null;
    public DateTime Updated { get; set; }
}

public partial class SiteSetting
{
    // starts with a letter, so a key can never be mistaken for an id; letters,
    // digits and dots only, with no empty segments
    private static readonly Regex KeyPattern =
        new(@"^[A-Za-z][A-Za-z0-9]*(\.[A-Za-z0-9]+)*\z");

    public static SiteSetting? ByKeyOrId(
        IContentContext context,
        string value)
    {
        if (long.TryParse(value, out var id))
            return context.SiteSettings?
                .SingleOrDefault(s => s.SettingId == id);

        return context.SiteSettings?
            .SingleOrDefault(s => s.Key == value);
    }

    public static bool IsValidKey(string? key) =>
        !string.IsNullOrEmpty(key) &&
        key.Length <= KeyMaxLength &&
        KeyPattern.IsMatch(key);

    /// <summary>
    /// Checks the fields a caller supplies, returning a message suitable for
    /// a 400 response, or null when the setting is valid.
    /// </summary>
    public static string? Validate(SiteSetting setting)
    {
        if (!IsValidKey(setting.Key))
            return $"Key must be 1 to {KeyMaxLength} characters: letters, " +
                   "digits and dots, starting with a letter, with no empty " +
                   "segments";
        if (setting.Description?.Length > DescriptionMaxLength)
            return $"Description must be at most {DescriptionMaxLength} " +
                   "characters";
        return null;
    }
}

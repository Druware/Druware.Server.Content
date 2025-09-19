namespace Druware.Server.Content;

// Wy GUID's?  Unique, marginally harder to resolve to a role, easy to hash
// for client side permissions checks in the UI

public static class NewsSecurityRole
{
    public const string Author = "50907F4E-4333-4B71-8D3E-94B918C83B5A";
    public const string Editor = "18738CE6-6CAC-4EB4-BFD4-D98BC55D751F";
    public const string AuthorOrEditor = Author + ", " + Editor;
}


public static class DocumentSecurityRole
{
    public const string Author = "B187BD0D-13A2-44FC-9C68-4D5B3F01CFCF";
    public const string Editor = "05E796FB-BD6A-48A0-A879-F7A1984306CA";
    public const string AuthorOrEditor = Author + ", " + Editor;
}

public static class AssetSecurityRole
{
    public const string Author = "40F6CE86-5B48-4AB0-A80A-360B6D1D427A";
    public const string Editor = "E5227C63-D628-4B46-AD3D-39509B37E17E";
    public const string AuthorOrEditor = Author + ", " + Editor;
}

public static class ProductSecurityRole
{
    public const string Author = "102BFDA8-D052-4181-9020-185914A213DD";
    public const string Editor = "FF0013D1-16FF-4A07-969E-B5F5D1F43F6A";
    public const string AuthorOrEditor = Author + ", " + Editor;
}
// TODO: Add additional Roles below

// NOTE: Role
// NOTE: AssetType uses the System Administrator Role

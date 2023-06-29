namespace Druware.Server.Content;

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
// TODO: Add additional Roles below

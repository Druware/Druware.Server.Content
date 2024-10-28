using Druware.Server.Content.Entities;
using Druware.Server.Content.Entities.Configuration;
using Druware.Server.Entities;
using Druware.Server.Entities.Configuration.Microsoft;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// NOTE: Migrations should ALWAYS be built with the platform specific Contexts

namespace Druware.Server.Content;

public interface IContentContext
{
    public DbSet<Article>? News { get; set; }
    public DbSet<ArticleTag>? ArticleTags { get; set; }
    
    public DbSet<AssetType>? AssetTypes { get; set; }
    public DbSet<Asset>? Assets { get; set; }
        
    public DbSet<Document>? Documents { get; set; }
    public DbSet<DocumentTag>? DocumentTags { get; set; }
    
    public DbSet<Product>? Products { get; set; }
    public DbSet<ProductTag>? ProductTags { get; set; }


}


public class ContentContext : DbContext, IContentContext
{
    private readonly IConfiguration? _configuration;

    public ContentContext()
    {
    }

    public ContentContext(IConfiguration? configuration) // : base()
    {
        _configuration = configuration;
    }

    public ContentContext(DbContextOptions<ContentContext> options,
        IConfiguration? configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Article>? News { get; set; }
    public DbSet<ArticleTag>? ArticleTags { get; set; }
    
    public DbSet<AssetType>? AssetTypes { get; set; }
    public DbSet<Asset>? Assets { get; set; }
        
    public DbSet<Document>? Documents { get; set; }
    public DbSet<DocumentTag>? DocumentTags { get; set; }

    public DbSet<Product>? Products { get; set; }
    public DbSet<ProductTag>? ProductTags { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        if (_configuration == null)
            throw new Exception(
                "The ContentContext has tried to create without a configuration");
        
        var settings = new AppSettings(_configuration!);
        switch (settings.DbType)
        {
            case DbContextType.Microsoft:
                if (settings.ConnectionString != null)
                    optionsBuilder.UseSqlServer(settings.ConnectionString);
                break;
            case DbContextType.PostgreSql:
                if (settings.ConnectionString != null)
                    optionsBuilder.UseNpgsql(settings.ConnectionString);
                break;
            case DbContextType.Sqlite:
                if (settings.ConnectionString != null)
                    optionsBuilder.UseSqlite(settings.ConnectionString);
                break;            
            default:
                throw new Exception(
                    "There is no configuration for this DbType");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new AssetTypeConfiguration());
        builder.ApplyConfiguration(new AssetConfiguration());

        // Though Tag is external, we need to ensure we have the config here too
        // Without this here, News and Documents fail because they cannot 
        // resolve the property names to the field names for the FK lookups.
        builder.ApplyConfiguration(new TagConfiguration());

        var settings = new AppSettings(_configuration!);
        switch (settings.DbType)
        {
            case DbContextType.Microsoft:
                builder.ApplyConfiguration(new Entities.Configuration.Microsoft.ArticleConfiguration());
                builder.ApplyConfiguration(new Entities.Configuration.Microsoft.DocumentConfiguration());
                builder.ApplyConfiguration(new Entities.Configuration.Microsoft.ProductConfiguration());
                builder.ApplyConfiguration(new Entities.Configuration.Microsoft.ProductReleaseConfiguration());
                break;
            case DbContextType.PostgreSql:
                builder.ApplyConfiguration(new Entities.Configuration.PostgreSql.ArticleConfiguration());
                builder.ApplyConfiguration(new Entities.Configuration.PostgreSql.DocumentConfiguration());
                builder.ApplyConfiguration(new Entities.Configuration.PostgreSql.ProductConfiguration());
                builder.ApplyConfiguration(new Entities.Configuration.PostgreSql.ProductReleaseConfiguration());
                break;
            case DbContextType.Sqlite:
                builder.ApplyConfiguration(new Entities.Configuration.Sqlite.ArticleConfiguration());
                builder.ApplyConfiguration(new Entities.Configuration.Sqlite.DocumentConfiguration());
                builder.ApplyConfiguration(new Entities.Configuration.Sqlite.ProductConfiguration());
                builder.ApplyConfiguration(new Entities.Configuration.Sqlite.ProductReleaseConfiguration());
                break;
            default:
                throw new Exception(
                    "There is no configuration for this DbType");        
        }
        builder.ApplyConfiguration(new ArticleTagConfiguration());
        
        builder.ApplyConfiguration(new DocumentTagConfiguration());
        
        builder.ApplyConfiguration(new ProductTagConfiguration());
    }

    public static void ConfigureSecurityRoles(ServerContext context)
    {
        if (context.Roles.FirstOrDefault<IdentityRole<string>>(r =>
                r.Name == NewsSecurityRole.Author) == null)
            context.Roles.Add(
                new Role
                {
                    Description = "News Author",
                    Name = NewsSecurityRole.Author,
                    NormalizedName = NewsSecurityRole.Author.ToUpper()
                });
        if (context.Roles.FirstOrDefault<IdentityRole<string>>(r =>
                r.Name == NewsSecurityRole.Editor) == null)
            context.Roles.Add(
                new Role
                {
                    Description = "News Editor",
                    Name = NewsSecurityRole.Editor,
                    NormalizedName = NewsSecurityRole.Editor.ToUpper()
                });

        if (context.Roles.FirstOrDefault<IdentityRole<string>>(r =>
                r.Name == DocumentSecurityRole.Author) == null)
            context.Roles.Add(
                new Role
                {
                    Description = "Document Author",
                    Name = DocumentSecurityRole.Author,
                    NormalizedName = DocumentSecurityRole.Author.ToUpper()
                });
        if (context.Roles.FirstOrDefault<IdentityRole<string>>(r =>
                r.Name == DocumentSecurityRole.Editor) == null)
            context.Roles.Add(
                new Role
                {
                    Description = "Document Editor",
                    Name = DocumentSecurityRole.Editor,
                    NormalizedName = DocumentSecurityRole.Editor.ToUpper()
                });

        if (context.Roles.FirstOrDefault<IdentityRole<string>>(r =>
                r.Name == AssetSecurityRole.Author) == null)
            context.Roles.Add(
                new Role
                {
                    Description = "Asset Author",
                    Name = AssetSecurityRole.Author,
                    NormalizedName = AssetSecurityRole.Author.ToUpper()
                });
        if (context.Roles.FirstOrDefault<IdentityRole<string>>(r =>
                r.Name == AssetSecurityRole.Editor) == null)
            context.Roles.Add(
                new Role
                {
                    Description = "Asset Editor",
                    Name = AssetSecurityRole.Editor,
                    NormalizedName = AssetSecurityRole.Editor.ToUpper()
                });
        
        if (context.Roles.FirstOrDefault<IdentityRole<string>>(r =>
                r.Name == ProductSecurityRole.Author) == null)
            context.Roles.Add(
                new Role
                {
                    Description = "Product Author",
                    Name = ProductSecurityRole.Author,
                    NormalizedName = ProductSecurityRole.Author.ToUpper()
                });
        if (context.Roles.FirstOrDefault<IdentityRole<string>>(r =>
                r.Name == ProductSecurityRole.Editor) == null)
            context.Roles.Add(
                new Role
                {
                    Description = "Product Editor",
                    Name = ProductSecurityRole.Editor,
                    NormalizedName = ProductSecurityRole.Editor.ToUpper()
                });


        context.SaveChanges();
    } 
}

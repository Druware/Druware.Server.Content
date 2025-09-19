using Druware.Server.Content.Entities;

using Druware.Server.Content.Entities.Configuration;
using Druware.Server.Content.Entities.Configuration.Sqlite;
using Druware.Server.Entities.Configuration.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Druware.Server.Content;

public class ContentContextSqlite : DbContext, IContentContext
{
    private readonly IConfiguration? _configuration;

    public ContentContextSqlite() { }

    public ContentContextSqlite(IConfiguration? configuration) =>
        _configuration = configuration;

    public ContentContextSqlite(DbContextOptions<ContentContextSqlite> options,
        IConfiguration? configuration) : base(options) =>
        _configuration = configuration;
    
    public DbSet<Article>? News { get; set; }
    public DbSet<ArticleTag>? ArticleTags { get; set; }
    
    public DbSet<AssetType>? AssetTypes { get; set; }
    public DbSet<Asset>? Assets { get; set; }
        
    public DbSet<Document>? Documents { get; set; }
    public DbSet<DocumentTag>? DocumentTags { get; set; }
    
    public DbSet<Product>? Products { get; set; }
    public DbSet<ProductTag>? ProductTags { get; set; }

    /// <summary>
    /// Configure the User Context to use the database as defined by the
    /// ApplicationSettings
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        if (_configuration != null)
        {
            var settings = new AppSettings(_configuration!);
            if (settings.ConnectionString != null)
                optionsBuilder.UseSqlServer(settings.ConnectionString);
            return;
        }

#if DEBUG
        // this is required to run any migration generation.  By default, we
        // leave it empty, and only populate it for generating migrations.
        const string cs = "Data Source=druware.db;Cache=Shared";
        optionsBuilder.UseSqlite(cs);
#endif

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Though Tag is external, we need to ensure we have the config here too
        // Without this here, News and Documents fail because they cannot 
        // resolve the property names to the field names for the FK lookups.
        builder.ApplyConfiguration(new TagConfiguration());
        
        builder.ApplyConfiguration(new AssetTypeConfiguration());
        builder.ApplyConfiguration(new AssetConfiguration());

        builder.ApplyConfiguration(new ArticleConfiguration());
        builder.ApplyConfiguration(new ArticleTagConfiguration());
        
        builder.ApplyConfiguration(new DocumentConfiguration());
        builder.ApplyConfiguration(new DocumentTagConfiguration());
        
        builder.ApplyConfiguration(new ProductConfiguration());
        builder.ApplyConfiguration(new ProductReleaseConfiguration());
        builder.ApplyConfiguration(new ProductTagConfiguration());

        
    }
}
    
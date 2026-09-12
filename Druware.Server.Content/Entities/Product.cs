

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Druware.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RESTfulFoundation.Server;

namespace Druware.Server.Content.Entities;

public partial class Product
{
    public Product()
    {
        ProductTags = new HashSet<ProductTag>();
        ProductMeta = new HashSet<ProductMeta>();
        CollectionProducts = new HashSet<CollectionProduct>();
    }

    private Product(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
        ProductTags = new HashSet<ProductTag>();
        ProductMeta = new HashSet<ProductMeta>();
        CollectionProducts = new HashSet<CollectionProduct>();
    }
    
    public long? ProductId { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? Short { get; set; } = null;
    public string? Summary { get; set; } = null;
    public string? Description { get; set; } = null;
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? Created { get; set; } = null;
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? Updated { get; set; } = null;
    public string? License { get; set; } = null;
    public string? DownloadUrl { get; set; } = null;
    public string? DocumentationUrl { get; set; } = null;
    public string? IconUrl { get; set; } = null;
    public string? AppStoreApple { get; set; } = null;
    public string? AppStoreMs { get; set; } = null;
    public string? AppStoreGoogle { get; set; } = null;
    public string? AppStoreAmazon { get; set; } = null;
    public string? DirectOsx { get; set; } = null;
    public string? DirectWinArm { get; set; } = null;
    public string? DirectWinX64 { get; set; } = null;
    public string? DirectGnome { get; set; } = null;
    public string? DirectKde { get; set; } = null;
    public string? DirectHaiku { get; set; } = null;
    public string? RepoGithub { get; set; } = null;
	
    // public ICollection<Asset>? Assets { get; set; } = null;

    private ILazyLoader LazyLoader { get; set; }
    
    private ICollection<ProductRelease>? _history;
    [JsonIgnore]
    public ICollection<ProductRelease>? History
    {
        get => LazyLoader.Load(this, ref _history);
        set => _history = value;
    }
    
    [JsonIgnore]
    public virtual ICollection<ProductTag> ProductTags { get; set; }

    [JsonIgnore]
    public virtual ICollection<ProductMeta> ProductMeta { get; set; }

    [JsonIgnore]
    public virtual ICollection<CollectionProduct> CollectionProducts { get; set; }

    private string[]? _tags = null;
    [NotMapped]
    public string[]? Tags {
        get
        {
            if (_tags != null) return _tags;
            if (ProductTags == null) return new List<string>().ToArray();

            // otherwise, build the result from the ArticleTags
            List<string> list = new();
            foreach (ProductTag at in ProductTags)
                if (at.Tag?.Name != null) list.Add(at.Tag!.Name);
            _tags = list.ToArray();
            return _tags;
        }
        set => _tags = value;        
    }

    private Dictionary<string, string>? _meta = null;
    [NotMapped]
    public Dictionary<string, string>? Meta
    {
        get
        {
            if (_meta != null) return _meta;
            if (ProductMeta == null) return new Dictionary<string, string>();

            Dictionary<string, string> list = new();
            foreach (ProductMeta pm in ProductMeta)
                if (pm.Property != null) list[pm.Property] = pm.Value ?? "";
            _meta = list;
            return _meta;
        }
        set => _meta = value;
    }

    private string[]? _collections = null;
    [NotMapped]
    public string[]? Collections {
        get
        {
            if (_collections != null) return _collections;
            if (CollectionProducts == null) return new List<string>().ToArray();

            // otherwise, build the result from the CollectionProducts
            List<string> list = new();
            foreach (CollectionProduct cp in CollectionProducts)
                if (cp.Collection?.Name != null) list.Add(cp.Collection!.Name);
            _collections = list.ToArray();
            return _collections;
        }
    }
}

public partial class Product
{
    public static Product? ByShortOrId(
        ContentContext context,
        string? lookup)
    {
        Product? r = null;
        if (int.TryParse(lookup, out var id))
            r = context.Products?
                .Include("ProductTags.Tag")
                //.Include("Product.History")
                .SingleOrDefault(t => t.ProductId == id);

        return r ??= context.Products?
            .SingleOrDefault(t => t.Short == lookup);
    }
        
    public static bool IsShortAvailable(
        ContentContext context,
        string lookup) =>
        ByShortOrId(context, lookup) == null;
    
}
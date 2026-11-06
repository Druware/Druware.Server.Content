

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

    /// <summary>
    /// A disabled product is hidden from everyone but product authors,
    /// product editors and system administrators.
    /// </summary>
    public bool IsDisabled { get; set; } = false;

    /// <summary>
    /// Where a moved or renamed product now lives: another product's short
    /// or id, or an absolute http or https URL. Null when it has not moved.
    /// </summary>
    public string? RedirectTo { get; set; } = null;

    /// <summary>
    /// The current short of the product RedirectTo names, filled in by
    /// ResolveRedirect, so a client can build the new address without a
    /// second call. Null when RedirectTo is null, is a URL, or names a
    /// product that no longer exists.
    /// </summary>
    [NotMapped]
    public string? RedirectShort { get; set; } = null;
	
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

    // whatever was last assigned to Meta, normally by a request body; kept
    // apart from _meta because the getter caches its projection there
    private Dictionary<string, string>? _submittedMeta = null;

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
        set => _meta = _submittedMeta = value;
    }

    /// <summary>
    /// The meta a request body carried, or null when it omitted meta or
    /// sent null. Meta itself never reads as null, because it falls back
    /// to the rows, and reading it does not change this, so this is how a
    /// controller tells "leave the meta alone" from "replace it".
    /// </summary>
    [JsonIgnore, NotMapped]
    public Dictionary<string, string>? SubmittedMeta => _submittedMeta;

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

    public const int RedirectToMaxLength = 2048;

    /// <summary>
    /// Trims a redirect and turns a blank one into null.
    /// </summary>
    public static string? NormalizeRedirectTo(string? redirectTo) =>
        string.IsNullOrWhiteSpace(redirectTo) ? null : redirectTo.Trim();

    private static bool IsWebAddress(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var url) &&
        (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps);

    /// <summary>
    /// Checks a product's normalized RedirectTo: it must be null, an
    /// absolute http or https URL, or the short or id of another existing
    /// product. Returns the problem as a short sentence for a 400 response,
    /// or null.
    /// </summary>
    /// <param name="context">The context to look the target up in</param>
    /// <param name="product">The product as submitted, for its RedirectTo
    /// and its (new) Short</param>
    /// <param name="productId">The stored product's id on an update, null
    /// on a create</param>
    public static string? ValidateRedirectTo(
        ContentContext context,
        Product product,
        long? productId = null)
    {
        var redirectTo = product.RedirectTo;
        if (redirectTo == null) return null;

        if (redirectTo.Length > RedirectToMaxLength)
            return $"Redirect must be at most {RedirectToMaxLength} characters";
        if (IsWebAddress(redirectTo)) return null;

        if (redirectTo == product.Short)
            return "A product cannot redirect to itself";
        var target = ByShortOrId(context, redirectTo);
        if (target == null)
            return "Redirect must be an existing product's short or id, or a full http or https address";
        if (productId != null && target.ProductId == productId)
            return "A product cannot redirect to itself";
        return null;
    }

    /// <summary>
    /// Fills in RedirectShort when RedirectTo names a product that exists.
    /// Only one step is followed; the target's own redirect is not.
    /// </summary>
    public static void ResolveRedirect(ContentContext context, Product product)
    {
        product.RedirectShort = null;
        if (product.RedirectTo == null || IsWebAddress(product.RedirectTo))
            return;
        product.RedirectShort = ByShortOrId(context, product.RedirectTo)?.Short;
    }
}
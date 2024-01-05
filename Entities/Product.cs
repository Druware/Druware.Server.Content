

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
    }

    private Product(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }
    
    public long? ProductId { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? Short { get; set; } = null;
    public string? Summary { get; set; } = null;
    public string? Description { get; set; } = null;
    public DateTime? Created { get; set; } = null;
    public DateTime? Updated { get; set; } = null;
    public string? License { get; set; } = null;
    public string? DownloadUrl { get; set; } = null;
    public string? DocumentationUrl { get; set; } = null;
    public string? IconUrl { get; set; } = null;
	
    // public ICollection<Asset>? Assets { get; set; } = null;

    private ILazyLoader LazyLoader { get; set; }
    
    private ICollection<ProductRelease>? _history;
    [JsonIgnore]
    public ICollection<ProductRelease>? History
    {
        get => LazyLoader.Load(this, ref _history);
        set => _history = value;
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
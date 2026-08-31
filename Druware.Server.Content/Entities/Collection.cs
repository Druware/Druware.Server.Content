

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Druware.Server.Content.Entities;

public partial class Collection
{
    public Collection()
    {
        CollectionProducts = new HashSet<CollectionProduct>();
    }

    public long? CollectionId { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? Description { get; set; } = null;
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? Created { get; set; } = null;
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? Updated { get; set; } = null;

    [JsonIgnore]
    public virtual ICollection<CollectionProduct> CollectionProducts { get; set; }

    private string[]? _products = null;
    [NotMapped]
    public string[]? Products {
        get
        {
            if (_products != null) return _products;
            if (CollectionProducts == null) return new List<string>().ToArray();

            // otherwise, build the result from the CollectionProducts
            List<string> list = new();
            foreach (CollectionProduct cp in CollectionProducts)
                if (cp.Product?.Short != null) list.Add(cp.Product!.Short);
            _products = list.ToArray();
            return _products;
        }
        set => _products = value;
    }
}

public partial class Collection
{
    public static Collection? ByNameOrId(
        ContentContext context,
        string? lookup)
    {
        Collection? r = null;
        if (long.TryParse(lookup, out var id))
            r = context.Collections?
                .Include("CollectionProducts.Product")
                .SingleOrDefault(c => c.CollectionId == id);

        return r ??= context.Collections?
            .SingleOrDefault(c => c.Name == lookup);
    }

    public static bool IsNameAvailable(
        ContentContext context,
        string lookup) =>
        ByNameOrId(context, lookup) == null;

}

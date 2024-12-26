namespace BookstoreManagement.Shared.Models;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Image { get; set; } = null!;

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public int? ProviderId { get; set; }

    public virtual ICollection<ImportItem> ImportItems { get; set; } = new List<ImportItem>();

    public virtual ICollection<InvoicesItem> InvoicesItems { get; set; } = new List<InvoicesItem>();

    public virtual ICollection<ItemPrice> ItemPrices { get; set; } = new List<ItemPrice>();

    public virtual Provider? Provider { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

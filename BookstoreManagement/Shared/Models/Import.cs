namespace BookstoreManagement.Shared.Models;

public partial class Import
{
    public int Id { get; set; }

    public int ProviderId { get; set; }

    public decimal TotalCost { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool Applied { get; set; } = false;

    public virtual ICollection<ImportItem> ImportItems { get; set; } = new List<ImportItem>();

    public virtual Provider Provider { get; set; } = null!;
}

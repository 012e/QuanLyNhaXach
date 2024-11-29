namespace BookstoreManagement.Shared.Models;

public partial class ImportItem
{
    public int ImportId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public virtual Import Import { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;
}

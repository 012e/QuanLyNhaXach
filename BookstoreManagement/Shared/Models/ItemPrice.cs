namespace BookstoreManagement.Shared.Models;

public partial class ItemPrice
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public string PriceType { get; set; } = null!;

    public decimal Percentage { get; set; }

    public int Ordering { get; set; }

    public DateOnly BeginDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual Item Item { get; set; } = null!;
}

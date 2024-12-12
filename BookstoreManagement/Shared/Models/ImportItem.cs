using CommunityToolkit.Mvvm.ComponentModel;

namespace BookstoreManagement.Shared.Models;

public partial class ImportItem : ObservableObject
{
    public int ImportId { get; set; }

    public int ItemId { get; set; }

    public int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public virtual Import Import { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;
}

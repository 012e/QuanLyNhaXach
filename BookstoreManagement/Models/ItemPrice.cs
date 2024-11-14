using System;
using System.Collections.Generic;

namespace BookstoreManagement.Models;

public partial class ItemPrice
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public int PriceType { get; set; }

    public int Divider { get; set; }

    public int Value { get; set; }

    public DateOnly BeginDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual PriceType PriceTypeNavigation { get; set; } = null!;
}

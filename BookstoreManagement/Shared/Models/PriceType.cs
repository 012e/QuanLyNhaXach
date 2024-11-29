﻿namespace BookstoreManagement.Shared.Models;

public partial class PriceType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ItemPrice> ItemPrices { get; set; } = new List<ItemPrice>();
}
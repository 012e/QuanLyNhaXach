using System;
using System.Collections.Generic;

namespace BookstoreManagement.Models;

public partial class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Image { get; set; } = null!;

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? ProviderId { get; set; }

    public virtual ICollection<InvoicesItem> InvoicesItems { get; set; } = new List<InvoicesItem>();

    public virtual Provider? Provider { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

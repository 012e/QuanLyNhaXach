using System;
using System.Collections.Generic;

namespace BookstoreManagement.Models;

public partial class Tag
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}

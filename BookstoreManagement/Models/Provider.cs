using BookstoreManagement.DbContexts;
using System;
using System.Collections.Generic;

namespace BookstoreManagement.Models;

public partial class Provider : ISoftDelete
{
    public int ProviderId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool Deleted { get; set; } = false;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}

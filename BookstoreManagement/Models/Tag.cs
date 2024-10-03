using BookstoreManagement.DbContexts;
using System;
using System.Collections.Generic;

namespace BookstoreManagement.Models;

public partial class Tag : ISoftDelete
{
    public int TagId { get; set; }
    public string Name { get; set; } = null!;

    public string Description { get; set; }
    public bool Deleted { get; set; } = false;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}

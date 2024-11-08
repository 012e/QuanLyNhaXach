﻿using System;
using System.Collections.Generic;

namespace BookstoreManagement.Models;

public partial class Provider
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Item> ItemsNavigation { get; set; } = new List<Item>();
}

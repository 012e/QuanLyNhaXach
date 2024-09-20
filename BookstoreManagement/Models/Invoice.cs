using BookstoreManagement.DbContexts;
using System;
using System.Collections.Generic;

namespace BookstoreManagement.Models;

public partial class Invoice : ISoftDelete
{
    public int InvoiceId { get; set; }

    public decimal Total { get; set; }

    public int EmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public bool Deleted { get; set; } = false;

    public virtual ICollection<InvoicesItem> InvoicesItems { get; set; } = new List<InvoicesItem>();
}

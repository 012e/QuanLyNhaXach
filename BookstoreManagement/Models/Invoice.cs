using System;
using System.Collections.Generic;

namespace BookstoreManagement.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public decimal Total { get; set; }

    public int EmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<InvoicesItem> InvoicesItems { get; set; } = new List<InvoicesItem>();
}

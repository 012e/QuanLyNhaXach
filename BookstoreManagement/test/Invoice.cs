using System;
using System.Collections.Generic;

namespace BookstoreManagement.test;

public partial class Invoice
{
    public int Id { get; set; }

    public decimal Total { get; set; }

    public int EmployeeId { get; set; }

    public int CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<InvoicesItem> InvoicesItems { get; set; } = new List<InvoicesItem>();
}

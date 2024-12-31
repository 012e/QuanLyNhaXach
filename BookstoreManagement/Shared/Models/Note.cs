using System;
using System.Collections.Generic;

namespace BookstoreManagement.Shared.Models;

public partial class Note
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly DueDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}

﻿using System;
using System.Collections.Generic;

namespace BookstoreManagement.test;

public partial class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public decimal Salary { get; set; }

    public string ProfilePicture { get; set; } = null!;

    public bool IsManager { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

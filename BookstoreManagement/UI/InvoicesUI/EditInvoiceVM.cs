﻿using BookstoreManagement.Core;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class EditInvoiceVM : ContextualViewModel<Invoice>
{
    public override Invoice ViewModelContext { get; set; }
}

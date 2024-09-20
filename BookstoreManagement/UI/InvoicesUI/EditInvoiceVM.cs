using BookstoreManagement.Core;
using BookstoreManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class EditInvoiceVM : ContextualViewModel<int>
{
    public override int ViewModelContext { get; set; }
}

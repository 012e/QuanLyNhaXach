using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class AllInvoicesVM : ListVM<Invoice, EditInvoiceVM, CreateInvoiceVM>
{
    protected override DbContext Db { get; }

    protected override IContextualNavigatorService<EditInvoiceVM, Invoice> EditItemNavigator { get; }

    protected override INavigatorService<CreateInvoiceVM> CreateItemNavigator { get; }

    public AllInvoicesVM(
        ApplicationDbContext db,
        IContextualNavigatorService<EditInvoiceVM, Invoice> editInvoiceNavigator,
        INavigatorService<CreateInvoiceVM> createInvoiceNavigator
        )
    {
        CreateItemNavigator = createInvoiceNavigator;
        EditItemNavigator = editInvoiceNavigator;
        Db = db;
    }
}

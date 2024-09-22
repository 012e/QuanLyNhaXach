using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class EditInvoiceVM : EditItemVM<Invoice>
{
    protected override ApplicationDbContext Db { get; }

    [ObservableProperty]
    private ObservableCollection<Item> _invoiceItems = new();
    
    public INavigatorService<AllInvoicesVM> AllInvoicesNavigator { get; }
    public IContextualNavigatorService<AddInvoiceItemVM, Invoice> AddInvoiceItemNavigator { get; }

    [ObservableProperty]
    private Invoice _invoice;


    [RelayCommand]
    private void GoBack()
    {
        AllInvoicesNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToAddInvoiceItem()
    {
        AddInvoiceItemNavigator.Navigate(Invoice);
    }

    public override void ResetState()
    {
        InvoiceItems = new();
        base.ResetState();
    }

    protected override void LoadItem()
    {
        Invoice = ViewModelContext;
        var itemsFromInvoice = from items in Db.Items
                    join invoiceItems in Db.InvoicesItems on items.ItemId equals invoiceItems.ItemId
                    where invoiceItems.InvoiceId == Invoice.InvoiceId select items;
        InvoiceItems = new ObservableCollection<Item>(itemsFromInvoice);
    }

    protected override void SubmitItemHandler()
    {
    }

    public EditInvoiceVM(ApplicationDbContext db,
        INavigatorService<AllInvoicesVM> allInvoicesNavigator,
        IContextualNavigatorService<AddInvoiceItemVM, Invoice> addInvoiceItemNavigator
        )
    {
        Db = db;
        AllInvoicesNavigator = allInvoicesNavigator;
        AddInvoiceItemNavigator = addInvoiceItemNavigator;
    }
}

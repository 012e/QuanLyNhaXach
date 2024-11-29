using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class EditInvoiceVM : EditItemVM<Invoice>
{
    private readonly ApplicationDbContext db;

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
        if (Invoice == null) return;
        AddInvoiceItemNavigator.Navigate(Invoice);
    }

    public override void ResetState()
    {
        base.ResetState();
        InvoiceItems = new();
    }

    protected override void LoadItem()
    {
        Invoice = ViewModelContext;
        var itemsFromInvoice = from items in db.Items
                               join invoiceItems in db.InvoicesItems on items.Id equals invoiceItems.ItemId
                               where invoiceItems.InvoiceId == Invoice.Id
                               select items;
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
        this.db = db;
        AllInvoicesNavigator = allInvoicesNavigator;
        AddInvoiceItemNavigator = addInvoiceItemNavigator;
    }
}

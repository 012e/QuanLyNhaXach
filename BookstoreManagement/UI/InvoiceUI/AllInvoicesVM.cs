using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class AllInvoicesVM : ListVM<Invoice, EditInvoiceVM>
{
    private readonly ApplicationDbContext db;
    protected override IContextualNavigatorService<EditInvoiceVM, Invoice> EditItemNavigator { get; }
    protected INavigatorService<CreateInvoiceVM> CreateInvoiceNavigator { get; }
    [ObservableProperty]
    private String _searchText = "";

    [RelayCommand]
    protected void NavigateToCreateInvoice()
    {
        CreateInvoiceNavigator.Navigate();
    }
    protected override bool FitlerItem(Invoice item)
    {
        string findString = item.EmployeeId.ToString() + item.CreatedAt.ToString() + item.CustomerId.ToString();
        return findString.ToLower().Contains(SearchText.ToLower());
    }
    partial void OnSearchTextChanged(string value)
    {
        ItemsView.Refresh();
    }

    private bool ConfirmDelete(Invoice invoice)
    {
        var result = MessageBox.Show("Are you sure you want to delete this invoice?", "Delete Invoice", MessageBoxButton.YesNo);
        return result == MessageBoxResult.Yes;
    }


    protected override void LoadItems()
    {
        db.ChangeTracker.Clear();
        var invoices = db.Invoices.OrderBy(invoice => invoice.Id).ToList();
        Items = new(invoices);
    }

    protected override void DeleteItem(Invoice invoice)
    {
        if (invoice is null)
        {
            return;
        }
        if (ConfirmDelete(invoice))
        {
            db.Invoices.Where(i => i.Id == invoice.Id).ExecuteDelete();
            db.SaveChanges();
            LoadItemsInBackground();
        }
    }

    public AllInvoicesVM(
        ApplicationDbContext db,
        IContextualNavigatorService<EditInvoiceVM, Invoice> editInvoiceNavigator,
        INavigatorService<CreateInvoiceVM> createInvoiceNavigator
        )
    {
        EditItemNavigator = editInvoiceNavigator;
        CreateInvoiceNavigator = createInvoiceNavigator;
        this.db = db;
    }
}

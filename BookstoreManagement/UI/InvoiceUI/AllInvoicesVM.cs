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

public partial class AllInvoicesVM : ListVM<Invoice, EditInvoiceVM>
{
    protected override DbContext Db { get; }
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
        string findString = item.EmployeeId.ToString() + item.CreatedAt.ToString() + item.CustomerId.ToString() ;
        return findString.ToLower().Contains(SearchText.ToLower());
    }
    partial void OnSearchTextChanged(string value)
    {
        ItemsView.Refresh();
    }
    public AllInvoicesVM(
        ApplicationDbContext db,
        IContextualNavigatorService<EditInvoiceVM, Invoice> editInvoiceNavigator,
        INavigatorService<CreateInvoiceVM> createInvoiceNavigator
        )
    {
        EditItemNavigator = editInvoiceNavigator;
        CreateInvoiceNavigator = createInvoiceNavigator;
        Db = db;
    }
}

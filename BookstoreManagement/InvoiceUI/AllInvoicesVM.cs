using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.InvoiceUI.Dtos;
using BookstoreManagement.PricingUI.Services;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using BookstoreManagement.UI.DashboardUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using ToastNotifications.Core;

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
        WarningNotification();
        var result = MessageBox.Show("Are you sure you want to delete this invoice?", "Delete Invoice", MessageBoxButton.YesNo);
        return result == MessageBoxResult.Yes;
    }


    [ObservableProperty]
    private ObservableCollection<InvoiceItemDto> _invoiceItemDto;

    [ObservableProperty]
    private Invoice _invoice;
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
            SuccessNotification();
            LoadItemsInBackground();
        }
    }


    private readonly PricingService pricingService;

    private decimal GetTotalPrice(int invoiceId)
    {
        decimal sum = 0;
        foreach(var total in db.InvoicesItems.Where(ii =>ii.InvoiceId == invoiceId))
        {
            sum += total.Quantity*pricingService.GetPrice(invoiceId).FinalPrice;
        }
        return sum;
    }
    private void SumTotalPriceInvoice(Invoice invoice)
    {
        decimal test = GetTotalPrice(invoice.Id);
        invoice.Total = GetTotalPrice(invoice.Id);
        db.Invoices.Update(invoice);
        db.SaveChanges();
    }

    public AllInvoicesVM(
        ApplicationDbContext db,
        IContextualNavigatorService<EditInvoiceVM, Invoice> editInvoiceNavigator,
        INavigatorService<CreateInvoiceVM> createInvoiceNavigator,
        PricingService pricingService
        )
    {
        EditItemNavigator = editInvoiceNavigator;
        CreateInvoiceNavigator = createInvoiceNavigator;
        this.db = db;
        this.pricingService = pricingService;   
    }

    private void WarningNotification()
    {
        GetNotification.NotifierInstance.WarningMessage("Warning", "This action cannot be undone", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
    private void SuccessNotification()
    {
        GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted item successfully", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
}

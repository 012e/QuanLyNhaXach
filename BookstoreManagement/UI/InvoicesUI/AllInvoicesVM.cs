using BookstoreManagement.Core;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class AllInvoicesVM : BaseViewModel
{
    private readonly ApplicationDbContext db;
    private readonly IContextualNavigatorService<EditInvoiceVM, int> editInvoiceNavigator;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private ObservableCollection<Invoice> _invoices = [];

    [ObservableProperty]
    private Boolean _canInteractWithTable = false;

    [ObservableProperty]
    private Boolean _canRefreshList = false;

    [RelayCommand]
    private void RefreshList()
    {
        Invoices.Clear();
        loadDataInBackground();
    }

    private void UpdateItems()
    {
        db.ChangeTracker.Clear();
        var invoices = db.Invoices.ToList();
        Invoices = new ObservableCollection<Invoice>(invoices);
    }

    private void BeginLoading()
    {
        IsLoading = true;
        CanInteractWithTable = false;
        CanRefreshList = false;
    }

    private void FinishLoading()
    {
        IsLoading = false;
        CanInteractWithTable = true;
        CanRefreshList = true;
    }

    private void loadDataInBackground()
    {
        // let 'em load my bruh
        if (IsLoading) return;

        BeginLoading();
        BackgroundWorker worker = new();

        worker.DoWork += (send, e) =>
        {
            UpdateItems();
        };

        worker.RunWorkerCompleted += (send, e) =>
        {
            FinishLoading();
            if (e.Error is not null)
            {
                MessageBox.Show($"Some error occured, couldn't fetch data: {e.Error}.");
            }
        };

        worker.RunWorkerAsync();
    }

    public override void ResetState()
    {
        // reset items
        Invoices = new ObservableCollection<Invoice>();
        loadDataInBackground();
        base.ResetState();
    }

    [RelayCommand]
    private void NavigateToEditInvoice(Invoice invoice)
    {
        if (invoice is null) return;
        editInvoiceNavigator.Navigate(invoice.InvoiceId);
    }


    [RelayCommand]
    private void DeleteInvoice(Invoice invoice)
    {
        db.Remove(invoice);
        try
        {
            db.SaveChanges();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Failed to delete item: {e}");
            return;
        }
        Invoices.Remove(invoice);
        MessageBox.Show("Deleted successfully");
    }

    public AllInvoicesVM(
        ApplicationDbContext db,
        IContextualNavigatorService<EditInvoiceVM, int> editInvoiceNavigator
    )
    {
        this.db = db;
        this.editInvoiceNavigator = editInvoiceNavigator;
    }
}

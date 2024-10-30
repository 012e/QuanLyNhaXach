using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace BookstoreManagement.UI.InvoicesUI;

// TODO: update total of invoice
// TODO: check for duplicated item
// TODO: add quantity (currently defaulted to 1)
public partial class AddInvoiceItemVM : EditItemVM<Invoice>
{
    protected override ApplicationDbContext Db { get; }
    public IContextualNavigatorService<EditInvoiceVM, Invoice> EditInvoiceNavigator { get; }

    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitItemCommand))]
    private Item _selectedItem = null;

    protected override void LoadItem()
    {
        Items = new ObservableCollection<Item>(Db.Items.ToList());
    }
    protected override bool CanSubmitItem => base.CanSubmitItem && SelectedItem is not null;

    [RelayCommand]
    private void GoBack()
    {
        EditInvoiceNavigator.Navigate(ViewModelContext);
    }

    public override void ResetState()
    {
        base.ResetState();
        Items = new();
    }

    protected override void SubmitItemHandler()
    {
        Db.InvoicesItems.Add(new InvoicesItem
        {
            InvoiceId = ViewModelContext.Id,
            ItemId = SelectedItem.Id,
            Quantity = 1
        });
        Db.SaveChanges();
    }

    protected override void OnSubmittingSuccess()
    {
        base.OnSubmittingSuccess();
        MessageBox.Show("Added your item");
    }

    public AddInvoiceItemVM(ApplicationDbContext db, IContextualNavigatorService<EditInvoiceVM, Invoice> editInvoiceNavigator)
    {
        Db = db;
        EditInvoiceNavigator = editInvoiceNavigator;
    }
}

using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace BookstoreManagement.UI.InvoicesUI;

// TODO: update total of invoice
// TODO: check for duplicated item
// TODO: add quantity (currently defaulted to 1)
public partial class AddInvoiceItemVM : EditItemVM<Invoice>
{
    private readonly ApplicationDbContext db;

    public IContextualNavigatorService<EditInvoiceVM, Invoice> EditInvoiceNavigator { get; }

    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitItemCommand))]
    private Item _selectedItem = null;

    protected override void LoadItem()
    {
        Items = new ObservableCollection<Item>(db.Items.ToList());
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
        db.InvoicesItems.Add(new InvoicesItem
        {
            InvoiceId = ViewModelContext.Id,
            ItemId = SelectedItem.Id,
            Quantity = 1
        });
        db.SaveChanges();
    }

    protected override void OnSubmittingSuccess()
    {
        base.OnSubmittingSuccess();
        MessageBox.Show("Added your item");
    }

    public AddInvoiceItemVM(ApplicationDbContext db, IContextualNavigatorService<EditInvoiceVM, Invoice> editInvoiceNavigator)
    {
        db = db;
        EditInvoiceNavigator = editInvoiceNavigator;
    }
}

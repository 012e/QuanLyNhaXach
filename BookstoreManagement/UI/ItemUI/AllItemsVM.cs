using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.Services;

namespace BookstoreManagement.UI.ItemUI;

public partial class AllItemsVM : BaseViewModel
{
    private readonly ApplicationDbContext db;
    private readonly IContextualNavigatorService<EditItemVM, int> editItemNavigator;
    private readonly INavigatorService<CreateItemVM> createItemNavigator;

    [ObservableProperty]
    private ObservableCollection<Item> _items = [];

    [ObservableProperty]
    private Boolean _isLoading = false;

    [ObservableProperty]
    private Boolean _canInteractWithTable = false;

    [ObservableProperty]
    private Boolean _canRefreshList = false;

    [RelayCommand]
    private void RefreshList()
    {
        Items.Clear();
        loadDataInBackground();
    }

    private void UpdateItems()
    {
        db.ChangeTracker.Clear();
        var items = db.Items.ToList();
        Items = new ObservableCollection<Item>(items);
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
        Items = new ObservableCollection<Item>();
        loadDataInBackground();
        base.ResetState();
    }

    [RelayCommand]
    private void NavigateToEditItem(Item item)
    {
        if (item is null) return;
        editItemNavigator.Navigate(item.ItemId);
    }

    [RelayCommand]
    private void NavigateToCreateItem()
    {
        createItemNavigator.Navigate();
    }

    [RelayCommand]
    private void DeleteItem(Item item)
    {
        db.Remove(item);
        try
        {
            db.SaveChanges();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Failed to delete item: {e}");
            return;
        }
        Items.Remove(item);
        MessageBox.Show("Deleted successfully");
    }

    public AllItemsVM(ApplicationDbContext db,
        IContextualNavigatorService<EditItemVM, int> editItemNavigator,
        INavigatorService<CreateItemVM> createItemNavigator
    )
    {
        this.db = db;
        this.editItemNavigator = editItemNavigator;
        this.createItemNavigator = createItemNavigator;
        loadDataInBackground();
    }
}

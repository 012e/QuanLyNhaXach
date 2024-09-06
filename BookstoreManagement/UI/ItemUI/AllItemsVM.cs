using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.Services;

namespace BookstoreManagement.UI.ItemUI;

public partial class AllItemsVM : BaseViewModel
{
    private readonly ApplicationDbContext db;
    private readonly IContextualNavigatorService<EditItemVM, Item> editItemNavigator;
    private readonly INavigatorService<CreateItemVM> createItemNavigator;

    [ObservableProperty]
    private ObservableCollection<Item> _items;

    [ObservableProperty]
    private Boolean _isLoading = false;

    [ObservableProperty]
    private Boolean _canInteractWithTable = false;

    private void updateItems()
    {
        db.ChangeTracker.Clear();
        var items = db.Items.ToList();
        Items = new ObservableCollection<Item>(items);
    }

    private void loadDataInBackground()
    {
        // let 'em load my bruh
        if (IsLoading) return;

        IsLoading = true;
        CanInteractWithTable = false;
        BackgroundWorker worker = new();

        worker.DoWork += (send, e) =>
        {
            Thread.Sleep(1000);
            updateItems();
        };

        worker.RunWorkerCompleted += (send, e) =>
        {
            IsLoading = false;
            CanInteractWithTable = true;
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
        editItemNavigator.Navigate(item);
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
        IContextualNavigatorService<EditItemVM, Item> editItemNavigator,
        INavigatorService<CreateItemVM> createItemNavigator
    )
    {
        this.db = db;
        this.editItemNavigator = editItemNavigator;
        this.createItemNavigator = createItemNavigator;
        loadDataInBackground();
    }
}

using BespokeFusion;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ToastNotifications.Core;
using ToastNotifications.Messages.Error;

namespace BookstoreManagement.UI.ItemUI;

public partial class AllItemsVM : ListVM<Item, EditItemVM>
{
    private readonly ApplicationDbContext db;
    protected override IContextualNavigatorService<EditItemVM, Item> EditItemNavigator { get; }
    protected INavigatorService<CreateItemVM> CreateItemNavigator { get; }

    [ObservableProperty]
    private String _searchText = "";

    [ObservableProperty]
    private ObservableCollection<Item> _listItems;

    partial void OnSearchTextChanged(string value)
    {
        ItemsView.Refresh();
    }

    [RelayCommand]
    protected void NavigateToCreateItem()
    {
        CreateItemNavigator.Navigate();
    }

    protected override bool FitlerItem(Item item)
    {
        return item.Name.ToLower().Contains(SearchText.ToLower());
    }


    public AllItemsVM(ApplicationDbContext db,
        IContextualNavigatorService<EditItemVM, Item> editItemNavigator,
        INavigatorService<CreateItemVM> createItemNavigator
    )
    {
        this.db = db;
        this.EditItemNavigator = editItemNavigator;
        this.CreateItemNavigator = createItemNavigator;
    }

    [RelayCommand]
    private void Test()
    {
        MessageBox.Show(SearchText);
    }

    protected override void LoadItems()
    {
        db.ChangeTracker.Clear();
        var items = db.Items.OrderBy(i => i.Id).Include(i => i.Tags).ToList();
        Items = new ObservableCollection<Item>(items);
    }
    

    private bool ConfirmDeleteItem(Item item)
    {
        WarningNotification();
        var result = MessageBox.Show($"Are you sure you want to delete {item.Name}?", "Delete Item", MessageBoxButton.YesNo,MessageBoxImage.Warning);
        return result == MessageBoxResult.Yes;
    }

    protected override void DeleteItem(Item item)
    {
        if (item == null)
        {
            return;
        }
        var itemToDelete = db.Items.Find(item.Id);
        if (itemToDelete == null)
        {
            return;
        }
        if (ConfirmDeleteItem(itemToDelete))
        {
            SuccessNotification();
            db.Items.Remove(itemToDelete);
            db.SaveChanges();
            LoadItemsInBackground();
        }

    }

    // Toast Nofification
    private void SuccessNotification()
    {
        GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted item successfully", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
    private void WarningNotification()
    {
        GetNotification.NotifierInstance.WarningMessage("Warning","This action cannot be undone", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
}
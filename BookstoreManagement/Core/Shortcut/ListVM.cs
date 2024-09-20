using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace BookstoreManagement.Core.Shortcut;


// # Exposes the following bindings
// - bool IsLoading
// - bool CanInteractWithTable
// - ObservableCollection<TItem> Items
//
// # Implements the following commands
// - void RefreshList()
// - void DeleteItem(TItem item)
// - void NavigateToEditItem(TItem item)
// - void NavigateToCreateItem()
public abstract partial class ListVM<TItem, TEditItemVM, TCreateItemVM> : BaseViewModel
    where TItem : class
    where TEditItemVM : ContextualViewModel<TItem>
    where TCreateItemVM : BaseViewModel
{
    // Your ef core DbContext
    protected abstract DbContext Db { get; }
    protected abstract IContextualNavigatorService<TEditItemVM, TItem> EditItemNavigator { get; }
    protected abstract INavigatorService<TCreateItemVM> CreateItemNavigator { get; }

    [RelayCommand]
    protected virtual void NavigateToEditItem(TItem item)
    {
        EditItemNavigator.Navigate(item);    
    }

    [RelayCommand]
    protected virtual void NavigateToCreateItem()
    {
        CreateItemNavigator.Navigate();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshListCommand))]
    [NotifyPropertyChangedFor(nameof(CanInteractWithTable))]
    protected bool _isLoading = false;
    public bool CanInteractWithTable => !IsLoading;

    [ObservableProperty]
    protected ObservableCollection<TItem> _items = [];

    private bool CanRefreshList => !IsLoading;
    [RelayCommand(CanExecute = nameof(CanRefreshList))]
    private void RefreshList()
    {
        Items.Clear();
        LoadDataInBackground();
    }

    protected virtual void UpdateItems()
    {
        Db.ChangeTracker.Clear();
        var items = Db.Set<TItem>().ToList();
        Items = new ObservableCollection<TItem>(items);
    }

    [RelayCommand]
    protected virtual void DeleteItem(TItem item)
    {
        Db.Remove(item);
        try
        {
            Db.SaveChanges();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Failed to delete item: {e}");
            return;
        }
        Items.Remove(item);
        MessageBox.Show("Deleted successfully");
    }

    private void BeginLoading()
    {
        IsLoading = true;
    }

    private void FinishLoading()
    {
        IsLoading = false;
    }

    private void LoadDataInBackground()
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
        Items = new ObservableCollection<TItem>();
        LoadDataInBackground();
        base.ResetState();
    }

}

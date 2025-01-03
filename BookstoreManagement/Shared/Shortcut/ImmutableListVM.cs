﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Windows;
using System.Windows.Data;

namespace BookstoreManagement.Core.Shortcut;


/// <summary>
/// Exposes the following bindings:
/// <list type="bullet">
///   <item><description>bool IsLoading</description></item>
///   <item><description>ObservableCollection&lt;TItem&gt; Items</description></item>
///   <item><description>ICollectionView ItemsView</description></item>
/// </list>
/// Implements the following commands:
/// <list type="bullet">
///   <item><description>void RefreshList()</description></item>
///   <item><description>void DeleteItem(TItem item)</description></item>
///   <item><description>void NavigateToEditItem(TItem item)</description></item>
///   <item><description>bool FilterItem</description></item>
/// </list>
/// </summary>
/// <typeparam name="TItem">Type of the resource.</typeparam>
/// <typeparam name="TEditItemVM">Type of the edit view model.</typeparam>
public abstract partial class ImmutableListVM<TItem> : BaseViewModel
    where TItem : class
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshListCommand))]
    protected bool _isLoading = false;

    [ObservableProperty]
    protected ObservableCollection<TItem> _items = [];

    [ObservableProperty]
    protected ICollectionView _itemsView = null;

    private bool CanRefreshList => !IsLoading;
    [RelayCommand(CanExecute = nameof(CanRefreshList))]
    private void RefreshList()
    {
        Items.Clear();
        LoadItemsInBackground();
    }

    private bool FilterItemInternal(object obj)
    {
        return FitlerItem((TItem)obj);
    }


    protected virtual bool FitlerItem(TItem item)
    {
        return true;
    }

    protected abstract void LoadItems();

    private void BeginLoading()
    {
        IsLoading = true;
    }

    private void FinishLoading()
    {
        IsLoading = false;
        ItemsView = CollectionViewSource.GetDefaultView(Items);
        ItemsView.Filter = FilterItemInternal;
    }

    protected void HandleException(Exception e)
    {
        if (e is DbException)
        {
            MessageBox.Show("Failed to connect to database, please check your connection.");
        }
        else
        {
            MessageBox.Show($"Some error occured, couldn't fetch data. Please refresh later.");
        }
    }

    protected void LoadItemsInBackground()
    {
        // let 'em load my bruh
        if (IsLoading) return;

        BeginLoading();
        BackgroundWorker worker = new();

        worker.DoWork += (send, e) =>
        {
            LoadItems();
        };

        worker.RunWorkerCompleted += (send, e) =>
        {
            FinishLoading();
            if (e.Error is not null)
            {
                HandleException(e.Error);
            }
        };

        worker.RunWorkerAsync();
    }

    public override void ResetState()
    {
        // reset items
        Items = new ObservableCollection<TItem>();
        ItemsView = CollectionViewSource.GetDefaultView(Items);
        ItemsView.Filter = FilterItemInternal;

        LoadItemsInBackground();
        base.ResetState();
    }

}

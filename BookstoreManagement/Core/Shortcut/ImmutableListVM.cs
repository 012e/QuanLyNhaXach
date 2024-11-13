using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using System.Windows;

namespace BookstoreManagement.Core.Shortcut;


/// <summary>
/// Exposes the following bindings:
/// <list type="bullet">
///   <item><description>bool IsLoading</description></item>
///   <item><description>ObservableCollection&lt;TItem&gt; Items</description></item>
/// </list>
/// Implements the following commands:
/// <list type="bullet">
///   <item><description>void RefreshList()</description></item>
///   <item><description>void DeleteItem(TItem item)</description></item>
///   <item><description>void NavigateToEditItem(TItem item)</description></item>
/// </list>
/// </summary>
/// <typeparam name="TItem">Type of the resource.</typeparam>
/// <typeparam name="TEditItemVM">Type of the edit view model.</typeparam>
public abstract partial class ImmutableListVM<TItem> : BaseViewModel
    where TItem : class
{
    // Your ef core DbContext
    protected abstract DbContext Db { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshListCommand))]
    protected bool _isLoading = false;

    [ObservableProperty]
    protected ObservableCollection<TItem> _items = [];

    private bool CanRefreshList => !IsLoading;
    [RelayCommand(CanExecute = nameof(CanRefreshList))]
    private void RefreshList()
    {
        Items.Clear();
        LoadDataInBackground();
    }

    protected virtual void LoadItems()
    {
        Db.ChangeTracker.Clear();
        var items = Db.Set<TItem>().ToList();
        Items = new ObservableCollection<TItem>(items);
    }

    private void BeginLoading()
    {
        IsLoading = true;
    }

    private void FinishLoading()
    {
        IsLoading = false;
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

    private void LoadDataInBackground()
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
        Db.ChangeTracker.Clear();
        // reset items
        Items = new ObservableCollection<TItem>();
        LoadDataInBackground();
        base.ResetState();
    }

}

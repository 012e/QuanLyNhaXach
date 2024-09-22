using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookstoreManagement.Core.Shortcut;

/// <summary>
/// Exposes the following bindings
/// <list type="bullet">
/// <item>
/// bool IsLoading
/// </item>
/// <item>
/// - ObservableCollection<TItem> Items
/// </item>
/// </list>
///
/// # Implements the following commands
/// - void RefreshList()
/// - void DeleteItem(TItem item)
/// - void NavigateToEditItem(TItem item)
/// </summary>
/// <typeparam name="TItem">Type of the resource</typeparam>
/// <typeparam name="TEditItemVM">Type of the edit view model</typeparam>
/// <typeparam name="TCreateItemVM">Type of the create view model</typeparam>
public abstract partial class ListVM<TItem, TEditItemVM> : ImmutableListVM<TItem>
    where TItem : class
    where TEditItemVM : BaseViewModel, IContextualViewModel<TItem>
{
    protected abstract IContextualNavigatorService<TEditItemVM, TItem> EditItemNavigator { get; }

    [RelayCommand]
    protected virtual void DeleteItem(TItem item)
    {
        if (item == null) return;
        var result = MessageBox.Show("Do you want to delete this item", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                Db.Set<TItem>().Remove(item);
                Db.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong. Couldn't delete item.", "Error");
            }
            Items.Remove(item);
            MessageBox.Show("Deleted your item successfully", "Deleted successfully");
        }
    }

    [RelayCommand]
    protected virtual void NavigateToEditItem(TItem item)
    {
        if (item == null) return;
        EditItemNavigator.Navigate(item);
    }
}

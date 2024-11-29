using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.Input;

namespace BookstoreManagement.Core.Shortcut;

/// <summary>
/// Exposes the following bindings:
/// </summary>
/// <typeparam name="TItem">Type of the resource.</typeparam>
/// <typeparam name="TEditItemVM">Type of the edit view model.</typeparam>
/// <remarks>
/// <list type="bullet">
///   <item><description>bool IsLoading</description></item>
///   <item><description>ObservableCollection&lt;TItem&gt; Items</description></item>
/// </list>
///
/// Implements the following commands:
/// <list type="bullet">
///   <item><description>void RefreshList()</description></item>
///   <item><description>void DeleteItem(TItem item)</description></item>
///   <item><description>void NavigateToEditItem(TItem item)</description></item>
/// </list>
/// </remarks>
public abstract partial class ListVM<TItem, TEditItemVM> : ImmutableListVM<TItem>
    where TItem : class
    where TEditItemVM : BaseViewModel, IContextualViewModel<TItem>
{
    protected abstract IContextualNavigatorService<TEditItemVM, TItem> EditItemNavigator { get; }

    [RelayCommand]
    protected abstract void DeleteItem(TItem item);

    [RelayCommand]
    protected virtual void NavigateToEditItem(TItem item)
    {
        if (item == null) return;
        EditItemNavigator.Navigate(item);
    }
}

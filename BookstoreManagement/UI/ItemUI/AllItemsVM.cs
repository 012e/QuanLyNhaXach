using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.Services;
using BookstoreManagement.Core.Shortcut;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using System.Windows.Data;

namespace BookstoreManagement.UI.ItemUI;

public partial class AllItemsVM : ListVM<Item, EditItemVM>
{
    protected override ApplicationDbContext Db { get; }
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
        this.Db = db;
        this.EditItemNavigator = editItemNavigator;
        this.CreateItemNavigator = createItemNavigator;
    }

    [RelayCommand]
    private void Test()
    {
        MessageBox.Show(SearchText);
    }
}

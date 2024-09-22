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

namespace BookstoreManagement.UI.ItemUI;

public partial class AllItemsVM : ListVM<Item, EditItemVM>
{
    protected override ApplicationDbContext Db { get; }
    protected override IContextualNavigatorService<EditItemVM, Item> EditItemNavigator { get; }
    protected INavigatorService<CreateItemVM> CreateItemNavigator { get; }

    [RelayCommand]
    protected void NavigateToCreateItem()
    {
        CreateItemNavigator.Navigate();
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

}

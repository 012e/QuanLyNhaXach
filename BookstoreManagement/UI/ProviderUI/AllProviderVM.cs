using System.Collections.ObjectModel;
using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.UI.ProviderUI;

public partial class AllProviderVM : ListVM<Provider, EditProviderVM>
{
    protected override IContextualNavigatorService<EditProviderVM, Provider> EditItemNavigator { get; }
    protected INavigatorService<CreateProviderVM> CreateProviderNavigator;
    protected override ApplicationDbContext Db { get; }
    [ObservableProperty]
    private string _searchText = "";
    protected override void LoadItems()
    {
        Db.ChangeTracker.Clear();
        var items = Db.Providers.OrderBy(provider => provider.Id).ToList();
        Items = new ObservableCollection<Provider>(items);
    }
    public AllProviderVM(ApplicationDbContext db , IContextualNavigatorService<EditProviderVM , Provider> editItemNavigator , 
        INavigatorService<CreateProviderVM> createProviderNavigator)
    {
        EditItemNavigator = editItemNavigator;
        CreateProviderNavigator = createProviderNavigator;
        Db = db;
    }
    protected override bool FitlerItem(Provider item)
    {
        return item.Name.ToLower().Contains(SearchText.ToLower());
    }
    partial void OnSearchTextChanged(string value)
    {
        ItemsView.Refresh();
    }
    [RelayCommand]
    protected void NavigateToCreateProvider()
    {
        CreateProviderNavigator.Navigate();
    }
}

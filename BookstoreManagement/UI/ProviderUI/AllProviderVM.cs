using System.Collections.ObjectModel;
using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.UI.ProviderUI;

public partial class AllProviderVM : ListVM<Provider, EditProviderVM>
{
    protected override IContextualNavigatorService<EditProviderVM, Provider> EditItemNavigator { get; }
    protected INavigatorService<CreateProviderVM> CreateProviderNavigator;
    protected override ApplicationDbContext Db { get; }

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
    [RelayCommand]
    protected void NavigateToCreateProvider()
    {
        CreateProviderNavigator.Navigate();
    }
}

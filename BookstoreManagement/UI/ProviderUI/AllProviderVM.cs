using System.Collections.ObjectModel;
using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.UI.ProviderUI;

public partial class AllProviderVM : ListVM<Provider, EditProviderVM>
{
    protected override IContextualNavigatorService<EditProviderVM, Provider> EditItemNavigator { get; }

    protected override ApplicationDbContext Db { get; }

    protected override void LoadItems()
    {
        Db.ChangeTracker.Clear();
        var items = Db.Providers.OrderBy(provider => provider.Id).ToList();
        Items = new ObservableCollection<Provider>(items);
    }
    public AllProviderVM(ApplicationDbContext db , IContextualNavigatorService<EditProviderVM , Provider> editItemNavigator)
    {
        EditItemNavigator = editItemNavigator;
        Db = db;
    }
}

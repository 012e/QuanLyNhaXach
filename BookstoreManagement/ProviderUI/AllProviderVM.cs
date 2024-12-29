using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using ToastNotifications.Core;

namespace BookstoreManagement.UI.ProviderUI;

public partial class AllProviderVM : ListVM<Provider, EditProviderVM>
{
    protected override IContextualNavigatorService<EditProviderVM, Provider> EditItemNavigator { get; }
    protected INavigatorService<CreateProviderVM> CreateProviderNavigator;
    private readonly ApplicationDbContext db;
    [ObservableProperty]
    private string _searchText = "";

    protected override void LoadItems()
    {
        db.ChangeTracker.Clear();
        var items = db.Providers.OrderBy(provider => provider.Id).ToList();
        Items = new ObservableCollection<Provider>(items);
    }

    public AllProviderVM(ApplicationDbContext db, IContextualNavigatorService<EditProviderVM, Provider> editItemNavigator,
        INavigatorService<CreateProviderVM> createProviderNavigator)
    {
        EditItemNavigator = editItemNavigator;
        CreateProviderNavigator = createProviderNavigator;
        this.db = db;
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


    private bool ConfirmDelete(Provider provider)
    {
        WarningNotification();
        var result = MessageBox.Show("Are you sure you want to delete this provider?", "Delete Invoice", MessageBoxButton.YesNo);
        return result == MessageBoxResult.Yes;
    }

    protected override void DeleteItem(Provider provider)
    {
        if (provider is null)
        {
            return;
        }
        if (ConfirmDelete(provider))
        {
            db.Providers.Where(i => i.Id == provider.Id).ExecuteDelete();
            db.SaveChanges();
            SuccessNotification();
            LoadItemsInBackground();
        }
    }
    private void WarningNotification()
    {
        GetNotification.NotifierInstance.WarningMessage("Warning", "This action cannot be undone", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
    private void SuccessNotification()
    {
        GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted provider successfully", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
}

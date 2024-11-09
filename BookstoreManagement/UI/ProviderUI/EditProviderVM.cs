using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.UI.ProviderUI;

public partial class EditProviderVM : EditItemVM<Provider>
{
    protected override ApplicationDbContext Db { get; }

    public INavigatorService<AllProviderVM> AllProvidersNavigator { get; set; }

    [ObservableProperty]
    private Provider _item;
    public EditProviderVM(ApplicationDbContext db , INavigatorService<AllProviderVM> allProvidersNavigator )
    {
        Db = db;
        AllProvidersNavigator = allProvidersNavigator;
    }
    [RelayCommand]
    private void NavigateBack()
    {
        AllProvidersNavigator.Navigate();
    }
    protected override void LoadItem()
    {
        var id = ViewModelContext.Id;
        Item = Db.Providers.Find(id);
    }

    protected override void SubmitItemHandler()
    {
        Db.Providers.Update(Item);
        Db.SaveChanges();
    }
    protected override void OnSubmittingSuccess()
    {
        base.OnSubmittingSuccess();
        MessageBox.Show("Submit successfully!");
    }
}

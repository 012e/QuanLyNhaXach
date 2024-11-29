using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookstoreManagement.UI.ProviderUI;

public partial class EditProviderVM : EditItemVM<Provider>
{
    private readonly ApplicationDbContext db;

    public INavigatorService<AllProviderVM> AllProvidersNavigator { get; set; }

    [ObservableProperty]
    private Provider _item;
    public EditProviderVM(ApplicationDbContext db, INavigatorService<AllProviderVM> allProvidersNavigator)
    {
        this.db = db;
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
        Item = db.Providers.Find(id);
    }

    protected override void SubmitItemHandler()
    {
        db.Providers.Update(Item);
        db.SaveChanges();
    }
    protected override void OnSubmittingSuccess()
    {
        base.OnSubmittingSuccess();
        MessageBox.Show("Submit successfully!");
    }
}

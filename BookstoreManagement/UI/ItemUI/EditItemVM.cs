using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace BookstoreManagement.UI.ItemUI;

public partial class EditItemVM : EditItemVM<Item>
{
    private readonly ApplicationDbContext db;

    public INavigatorService<AllItemsVM> AllItemsNavigator { get; }

    [ObservableProperty]
    private Item _item;

    public EditItemVM(
        ApplicationDbContext db,
        INavigatorService<AllItemsVM> allItemsNavigator)
    {
        this.db = db;
        AllItemsNavigator = allItemsNavigator;
    }

    [RelayCommand]
    private void NavigateBack()
    {
        AllItemsNavigator.Navigate();
    }

    public override void ResetState()
    {
        base.ResetState();
        Item = default;
    }

    protected override void LoadItem()
    {
        db.ChangeTracker.Clear();
        var itemId = ViewModelContext.Id;
        Item = db.Items.Include(item => item.Tags).First();
    }

    protected override void OnSubmittingSuccess()
    {
        base.OnSubmittingSuccess();
        MessageBox.Show("Submitted successfully");
    }

    protected override void SubmitItemHandler()
    {
        db.Items.Update(Item);
        db.SaveChanges();
    }
}

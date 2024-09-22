using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.Services;
using BookstoreManagement.Core.Shortcut;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.UI.ItemUI;

public partial class EditItemVM(
    ApplicationDbContext db,
    INavigatorService<AllItemsVM> allItemsNavigator)
    : EditItemVM<Item>
{
    protected override ApplicationDbContext Db => db;

    public INavigatorService<AllItemsVM> AllItemsNavigator { get; } = allItemsNavigator;

    [ObservableProperty]
    private Item _item;

    [RelayCommand]
    private void NavigateBack()
    {
        AllItemsNavigator.Navigate();
    }

    protected override void LoadItem()
    {
        Item = ViewModelContext;
    }

    protected override void OnSubmittingSuccess()
    {
        MessageBox.Show("Submitted successfully");
        base.OnSubmittingSuccess();
    }

    protected override void SubmitItemHandler()
    {
        Db.Items.Update(Item);
        Db.SaveChanges();
    }
}

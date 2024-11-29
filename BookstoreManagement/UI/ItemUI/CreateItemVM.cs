using BookstoreManagement.Core;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookstoreManagement.UI.ItemUI;

public partial class CreateItemVM : BaseViewModel
{
    private readonly ApplicationDbContext db;
    private readonly INavigatorService<AllItemsVM> allItemsNavigator;

    [ObservableProperty]
    private Item _item = new()
    {
        Image = ""
    };

    [RelayCommand]
    private void Submit()
    {
        try
        {
            db.Items.Add(Item);
            db.SaveChanges();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Couldn't add item: {e}");
            return;
        }
        MessageBox.Show("Added item successfully");
    }

    private void ResetToDefaultValues()
    {
        Item = new Item
        {
            Name = "",
            Image = "",
            Description = ""
        };
    }

    public override void ResetState()
    {
        ResetToDefaultValues();
        base.ResetState();
    }

    [RelayCommand]
    private void NavigateBack()
    {
        allItemsNavigator.Navigate();
    }

    public CreateItemVM(ApplicationDbContext db, INavigatorService<AllItemsVM> allItemsNavigator)
    {
        ResetToDefaultValues();
        this.db = db;
        this.allItemsNavigator = allItemsNavigator;
    }
}

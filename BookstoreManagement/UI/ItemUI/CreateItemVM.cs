using BookstoreManagement.Core;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookstoreManagement.UI.ItemUI;

public partial class CreateItemVM : BaseViewModel
{
    private readonly ApplicationDbContext db;
    private readonly INavigatorService<AllItemsVM> allItemsNavigator;

    [ObservableProperty]
    private String _name;

    [ObservableProperty]
    private String _description;

    [ObservableProperty]
    private decimal _price;

    [ObservableProperty]
    private int _quantity;

    [RelayCommand]
    private void Submit()
    {
        var item = new Item
        {
            Name = Name,
            Description = Description,
            Price = Price,
            Quantity = Quantity,
            Image = []
        };
        try
        {
            db.Add(item);
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
        Name = "Item name";
        Description = "Item description";
        Price = 0;
        Quantity = 0;
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

    public CreateItemVM(ApplicationDbContext db,INavigatorService<AllItemsVM> allItemsNavigator)
    {
        ResetToDefaultValues();
        this.db = db;
        this.allItemsNavigator = allItemsNavigator;
    }
}

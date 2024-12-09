using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BookstoreManagement.UI.ItemUI;

public partial class EditItemVM : EditItemVM<Item>
{
    private readonly ApplicationDbContext db;

    public INavigatorService<AllItemsVM> AllItemsNavigator { get; }

    [ObservableProperty]
    private Item _item;
    [ObservableProperty]
    private BitmapImage _imageSource;

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
    }

    protected override void LoadItem()
    {
        db.ChangeTracker.Clear();
        var itemId = ViewModelContext.Id;
        Item = db.Items
            .Include(item => item.Tags).Where(item => item.Id == itemId).First();
        if (Item == null)
        {
            MessageBox.Show("Item not found.");
            return;
        }

        if (!string.IsNullOrEmpty(Item.Image))
        {
            LoadImageFromUrl(Item.Image);
        }
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
    private void LoadImageFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            ImageSource = null; 
            return;
        }

        try
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url, UriKind.Absolute);
                bitmap.EndInit();
                ImageSource = bitmap; 
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading image: {ex.Message}");
        }
    }
}

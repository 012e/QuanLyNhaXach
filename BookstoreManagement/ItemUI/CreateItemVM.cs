using BookstoreManagement.Core;
using BookstoreManagement.ItemUI.Dtos;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BookstoreManagement.UI.ItemUI;

public partial class CreateItemVM : BaseViewModel
{
    private readonly ApplicationDbContext db;
    private readonly ImageUploader imageUploader;
    private readonly INavigatorService<AllItemsVM> allItemsNavigator;

    [ObservableProperty]
    private Item _item = new()
    {
        Image = ""
    };
    [ObservableProperty]
    private BitmapImage _imageSource;
    [ObservableProperty]
    private ObservableCollection<Tag> _listTags;
    [ObservableProperty]
    private ObservableCollection<ItemTagDto> _tags;
    [ObservableProperty]
    private bool _isSet = false;
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _imagePath = "";
    [RelayCommand]
    private void ImportImage()
    {

        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files|*.jpg;*.png";
        openFileDialog.FilterIndex = 1;
        if (openFileDialog.ShowDialog() == true)
        {
            ImagePath = openFileDialog.FileName;
            LoadImageFromUrl(ImagePath);
        }
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

    [RelayCommand]
    private void Submit()
    {
        try
        {
            if (!Check_Valid_Input())
            {
                MessageBox.Show(ErrorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Task.Run(async () =>
            {
                Item.Image = await imageUploader.ReplaceImageAsync(Item.Image, ImagePath);
                await db.SaveChangesAsync();
            });
            db.Items.Add(Item);
            db.SaveChanges();
            MessageBox.Show("Added item successfully");
            allItemsNavigator.Navigate();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Couldn't add item: Database Error" , "Error" , MessageBoxButton.OK , MessageBoxImage.Error);
            return;
        }
    }
    private bool Check_Valid_Input()
    {
        if (string.IsNullOrWhiteSpace(Item.Name))
        {
            ErrorMessage = "Item name can not be empty!";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Item.Description))
        {
            ErrorMessage = "Item description can not be empty!";
            return false;
        }
        if(Item.Quantity < 0)
        {
            ErrorMessage = "Quantity must be a non-negative integer!";
            return false;
        }
        ErrorMessage = string.Empty;
        return true;
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
        IsSet = false;  
        Tags = new ObservableCollection<ItemTagDto>();
        ListTags = new ObservableCollection<Tag>();
        var allTags = db.Tags.ToHashSet();
        foreach(var tag in allTags)
        {
            var itemTagDto = new ItemTagDto()
            {
                Tag = tag,
                IsChecked = false
            };
            itemTagDto.PropertyChanged += Tag_PropertyChanged;
            Tags.Add(itemTagDto);
        }
        
        base.ResetState();
    }
    [RelayCommand]
    private void OpenSetTag()
    {
       IsSet = true;
    }
    [RelayCommand]
    private void CloseSetTag()
    {
        IsSet = false;
    }
    [RelayCommand]
    private void NavigateBack()
    {
        allItemsNavigator.Navigate();
    }

    public CreateItemVM(ApplicationDbContext db, INavigatorService<AllItemsVM> allItemsNavigator, ImageUploader imageUploader)
    {
        ResetToDefaultValues();
        this.db = db;
        this.allItemsNavigator = allItemsNavigator;
        this.imageUploader = imageUploader;
    }
    private void Tag_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ItemTagDto.IsChecked))
        {
            var itemTag = sender as ItemTagDto;

            if (itemTag.IsChecked)
            {

                if (!ListTags.Any(t => t.Name == itemTag.Tag.Name))
                {
                    ListTags.Add(itemTag.Tag);
                    Item.Tags.Add(itemTag.Tag);
                }
            }
            else
            {

                var tagToRemove = ListTags.FirstOrDefault(t => t.Name == itemTag.Tag.Name);
                if (tagToRemove != null)
                {
                    ListTags.Remove(tagToRemove);
                    Item.Tags.Remove(itemTag.Tag);
                }
            }
        }
    }
}

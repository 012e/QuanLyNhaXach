﻿using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.ObjectModel;
using BookstoreManagement.UI.TagUI;
using BookstoreManagement.ItemUI.Dtos;

namespace BookstoreManagement.UI.ItemUI;

public partial class EditItemVM : EditItemVM<Item>
{
    private readonly ApplicationDbContext db;

    public INavigatorService<AllItemsVM> AllItemsNavigator { get; }

    [ObservableProperty]
    private Item _item;
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
    private bool _isSubmitSuccess = false;

    public EditItemVM(
        ApplicationDbContext db,
        INavigatorService<AllItemsVM> allItemsNavigator,
        INavigatorService<CreateTagVM> createTagNavigator)
    {
        this.db = db;
        AllItemsNavigator = allItemsNavigator;
        Item = new Item();
    }

    [RelayCommand]
    private void NavigateBack()
    {
        AllItemsNavigator.Navigate();
    }

    public override void ResetState()
    {
        IsSet = false;
        base.ResetState();
        
    }
    private bool Check_Valid_Input()
    {
        if (string.IsNullOrWhiteSpace(Item.Name))
        {
            ErrorMessage = "Item Name is empty!";
            return false;
        }
        if(string.IsNullOrWhiteSpace(Item.Description))
        {
            ErrorMessage = "Item description is empty!";
            return false;
        }
        if(Item.Quantity < 0)
        {
            ErrorMessage = "Item quantity must be a non-negative integer!";
            return false;
        }
        ErrorMessage = string.Empty;
        return true;
    }

    protected override void LoadItem()
    {
        Item = default;
        db.ChangeTracker.Clear();
        var itemId = ViewModelContext.Id;
        Item = db.Items
            .Include(item => item.Tags).Where(item => item.Id == itemId).First();
        var allTags = db.Tags.ToHashSet();
        var itemTags = Item.Tags.ToHashSet();

        Tags = new ObservableCollection<ItemTagDto>();
         
        foreach (var tag in allTags)
        {
            var exists = itemTags.Contains(tag);
            var itemTagDto = new ItemTagDto()
            {
                Tag = tag,
                IsChecked = exists
            };

            
            itemTagDto.PropertyChanged += Tag_PropertyChanged;

            Tags.Add(itemTagDto);

        }

        if (Item != null)
        {
            ListTags = new ObservableCollection<Tag>(Item.Tags);
        }
        else
        {
            ListTags = new ObservableCollection<Tag>();
            MessageBox.Show("item is null");
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
        if (IsSubmitSuccess)
        {
            MessageBox.Show("Submitted successfully");
            IsSubmitSuccess = false;
        }
        return;
    }

    protected override void SubmitItemHandler()
    {
        if (!Check_Valid_Input())
        {
            MessageBox.Show(ErrorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        db.Items.Update(Item);
        db.SaveChanges();
        IsSubmitSuccess = true;
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
    private void OpenSetTag()
    {
        IsSet = true;
    }
    
    [RelayCommand]
    private void CloseSetTag()
    {
        IsSet = false;
        
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

using BookstoreManagement.Core;
using BookstoreManagement.ItemUI.Dtos;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.ObjectModel;
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
    [ObservableProperty]
    private ObservableCollection<Tag> _listTags;
    [ObservableProperty]
    private ObservableCollection<ItemTagDto> _tags;
    [ObservableProperty]
    private bool _isSet = false;

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

    public CreateItemVM(ApplicationDbContext db, INavigatorService<AllItemsVM> allItemsNavigator)
    {
        ResetToDefaultValues();
        this.db = db;
        this.allItemsNavigator = allItemsNavigator;
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

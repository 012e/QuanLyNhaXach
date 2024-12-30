using BookstoreManagement.Core.Shortcut;
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
using BookstoreManagement.Shared.CustomMessages;
using ToastNotifications.Core;
using Microsoft.Win32;

namespace BookstoreManagement.UI.ItemUI;

public partial class EditItemVM : EditItemVM<Item>
{
    private readonly ApplicationDbContext db;
    private readonly ImageUploader imageUploader;

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
            LoadImageFromUrl(imageUploader.GetPublicUrl(ImagePath));
        }
    }

    public EditItemVM(
        ApplicationDbContext db,
        INavigatorService<AllItemsVM> allItemsNavigator,
        INavigatorService<CreateTagVM> createTagNavigator,
        ImageUploader imageUploader)
    {
        this.db = db;
        AllItemsNavigator = allItemsNavigator;
        this.imageUploader = imageUploader;
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
        ImageSource = null;
        base.ResetState();
    }

    private bool Check_Valid_Input()
    {
        if (string.IsNullOrWhiteSpace(Item.Name))
        {
            ErrorMessage = "Item Name is empty!";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Item.Description))
        {
            ErrorMessage = "Item description is empty!";
            return false;
        }
        if (Item.Quantity < 0)
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
            GetNotification.NotifierInstance.WarningMessage("Warning", "Item is null", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
            return;
        }

        ImagePath = Item.Image;
        if (!string.IsNullOrEmpty(Item.Image))
        {
            LoadImageFromUrl(imageUploader.GetPublicUrl(ImagePath));
        }
    }

    protected override void SubmitItemHandler()
    {
        if (!Check_Valid_Input())
        {
            ErrorNotification();
            return;
        }
        Task.Run(async () =>
        {
            Item.Image = await imageUploader.ReplaceImageAsync(Item.Image, ImagePath);
            await db.SaveChangesAsync();
        });
        db.Items.Update(Item);
        db.SaveChanges();
        IsSubmitSuccess = true;
    }

    protected override void OnSubmittingSuccess()
    {
        base.OnSubmittingSuccess();
        if (IsSubmitSuccess)
        {
            SuccessNotification();
            IsSubmitSuccess = false;
            AllItemsNavigator.Navigate();
        }
        return;
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
    private void SuccessNotification()
    {
        GetNotification.NotifierInstance.SuccessMessage("Success", "Submit successfully", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
    private void ErrorNotification()
    {
        GetNotification.NotifierInstance.ErrorMessage("Error", ErrorMessage, NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
}

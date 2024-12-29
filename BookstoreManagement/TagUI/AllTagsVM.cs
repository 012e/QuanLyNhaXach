using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using ToastNotifications.Core;

namespace BookstoreManagement.UI.TagUI
{
    public partial class AllTagsVM : ListVM<Tag, EditTagVM>
    {
        private readonly INavigatorService<CreateTagVM> createItemNavigator;
        [ObservableProperty]
        private string _searchText = "";
        protected override IContextualNavigatorService<EditTagVM, Tag> EditItemNavigator { get; }

        private readonly ApplicationDbContext db;

        [RelayCommand]
        private void NavigateToCreateItem()
        {
            createItemNavigator.Navigate();
        }

        public AllTagsVM(
            ApplicationDbContext db,
            IContextualNavigatorService<EditTagVM, Tag> editItemNavigator,
            INavigatorService<CreateTagVM> createItemNavigator
            )
        {
            this.db = db;
            EditItemNavigator = editItemNavigator;
            this.createItemNavigator = createItemNavigator;
        }

        protected override bool FitlerItem(Tag item)
        {
            return item.Name.ToLower().Contains(SearchText.ToLower());
        }

        partial void OnSearchTextChanged(string value)
        {
            ItemsView.Refresh();
        }

        protected override void LoadItems()
        {
            db.ChangeTracker.Clear();
            var tags = db.Tags.OrderBy(tag => tag.Id).ToList();
            Items = new(tags);
        }

        private bool ConfirmDelete(Tag tag)
        {
            WarningNotification();
            var result = MessageBox.Show($"Are you sure you want to delete {tag.Name}?", "Delete Tag", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        protected override void DeleteItem(Tag tag)
        {
            if (tag is null)
            {
                return;
            }
            if (ConfirmDelete(tag))
            {
                db.Tags.Where(e => e.Id == tag.Id).ExecuteDelete();
                SuccessNotification();
                db.SaveChanges();
                LoadItemsInBackground();
            }
        }
        private void WarningNotification()
        {
            GetNotification.NotifierInstance.WarningMessage("Warning", "This action cannot be undone", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
        private void SuccessNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted tag successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
    }
}

using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;

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
                db.SaveChanges();
                LoadItemsInBackground();
            }

        }
    }
}

using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using ToastNotifications.Core;

namespace BookstoreManagement.ImportUI
{
    public partial class AllImportVM : ListVM<Import, EditImportVM>
    {
        private readonly ApplicationDbContext db;
        protected override IContextualNavigatorService<EditImportVM, Import> EditItemNavigator { get; }
        protected INavigatorService<CreateImportVM> createImportNavigator { get; }
        public AllImportVM(ApplicationDbContext db,
            IContextualNavigatorService<EditImportVM, Import> editImportNavigator,
            INavigatorService<CreateImportVM> createImportNavigator)
        {
            this.db = db;
            this.EditItemNavigator = editImportNavigator;
            this.createImportNavigator = createImportNavigator;

        }
        [ObservableProperty]
        private string _searchText = "";


        [RelayCommand]
        protected void NavigateToCreateImport()
        {
            createImportNavigator.Navigate();
        }

        // Confirm Delete 
        private bool ConfirmDelete(Import import)
        {
            WarningNotification();
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this customer?", "Delete Customer", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        // Delete Import
        protected override void DeleteItem(Import import)
        {
            if (import is null)
            {
                return;
            }
            if (ConfirmDelete(import))
            {
                db.Imports.Remove(import);
                db.SaveChanges();
                SuccessNotification();
                LoadItemsInBackground();
            }
        }

        // Load Import
        protected override void LoadItems()
        {
            db.ChangeTracker.Clear();
            var items = db.Imports.OrderBy(import => import.Id).ToList();
            Items = new ObservableCollection<Import>(items);
        }

        // Filer Import
        protected override bool FitlerItem(Import item)
        {
            string itemId = item.Id.ToString();
            string providerID = item.ProviderId.ToString();
            return itemId.Contains(SearchText) || providerID.Contains(SearchText);
        }
        partial void OnSearchTextChanged(string value)
        {
            ItemsView.Refresh();
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
            GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted import successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
    }
}

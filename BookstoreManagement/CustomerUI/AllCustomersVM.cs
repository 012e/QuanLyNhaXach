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

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class AllCustomersVM : ListVM<Customer, EditCustomerVM>
    {
        private readonly ApplicationDbContext db;
        protected override IContextualNavigatorService<EditCustomerVM, Customer> EditItemNavigator { get; }
        protected INavigatorService<CreateCustomerVM> CreateCustomerNavigator { get; }
        [ObservableProperty]
        private string _searchText = "";
        public AllCustomersVM(ApplicationDbContext db,
            IContextualNavigatorService<EditCustomerVM, Customer> editCustomerNavigator,
            INavigatorService<CreateCustomerVM> createCustomerNavigator)
        {
            this.db = db;
            this.EditItemNavigator = editCustomerNavigator;
            this.CreateCustomerNavigator = createCustomerNavigator;
        }

        protected override void LoadItems()
        {
            db.ChangeTracker.Clear();
            var items = db.Customers.OrderBy(customer => customer.Id).ToList();
            Items = new ObservableCollection<Customer>(items);
        }

        [RelayCommand]
        private void NavigateToCreateCustomer()
        {
            CreateCustomerNavigator.Navigate();
        }

        protected override bool FitlerItem(Customer item)
        {
            string name = item.FirstName + item.LastName;
            return name.ToLower().Contains(SearchText.ToLower());
        }

        partial void OnSearchTextChanged(string value)
        {
            ItemsView.Refresh();
        }

        private bool ConfirmDelete(Customer customer)
        {
            WarningNotification();
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this customer?", "Delete Customer", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        protected override void DeleteItem(Customer customer)
        {
            if (customer is null)
            {
                return;
            }
            if (ConfirmDelete(customer))
            {
                db.Customers.Remove(customer);
                db.SaveChanges();
                SuccessNotification();
                LoadItemsInBackground();
            }
        }
        private void SuccessNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted customer successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }

        private void WarningNotification()
        {
            GetNotification.NotifierInstance.WarningMessage("Warning", "This action cannot be undone", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }
    }
}

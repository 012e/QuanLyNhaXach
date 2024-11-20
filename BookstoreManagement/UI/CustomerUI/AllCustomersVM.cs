using BookstoreManagement.Core;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class AllCustomersVM : BaseViewModel
    {
        // database
        private readonly ApplicationDbContext db;

        [ObservableProperty]
        private ObservableCollection<Customer> _customers; // list customer


        // navigator chuyen den edit
        private readonly IContextualNavigatorService<EditCustomerVM, int> editCustomerNavigator;

        // Navigator chuyen den CreateCustomer
        protected INavigatorService<CreateCustomerVM> CreateCustomerNavigator { get; }

        public AllCustomersVM(ApplicationDbContext db,
            IContextualNavigatorService<EditCustomerVM,int> editCustomerNavigator,
            INavigatorService<CreateCustomerVM> createCustomerNavigator)
        {
            this.db = db;
            this.editCustomerNavigator = editCustomerNavigator;
            this.CreateCustomerNavigator = createCustomerNavigator;
        }


        // load du lieu
        public override void ResetState()
        {
            base.ResetState();
            var customers = db.Customers.AsNoTracking().ToList();
            Customers = new(customers);
        }

        // nut chuyen den submit
        [RelayCommand]
        private void NavigateToCreateCustomer()
        {
            CreateCustomerNavigator.Navigate();
        }

        // xoa
        [RelayCommand]
        private void DeleteCustomer(Customer customer)
        {
            if (customer == null) return;

            MessageBoxResult result = MessageBox.Show(
                "Ban co chac muon xoa khach hang nay khong?",
                "Xac nhan xoa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                Customers.Remove(customer);
            }            
        }

        // chinh sua
        [RelayCommand]
        private void EditCustomer(Customer customer)
        {
            if (customer == null) return;
            editCustomerNavigator.Navigate(customer.Id);
        }

        // Refresh du lieu
        [RelayCommand]
        private void Refresh()
        {
            ResetState();
        }
    }
}

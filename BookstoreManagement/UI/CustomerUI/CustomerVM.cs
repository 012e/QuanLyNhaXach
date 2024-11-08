using BookstoreManagement.Core;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class CustomerVM : BaseViewModel
    {
        // database
        private readonly ApplicationDbContext db;

        [ObservableProperty]
        private ObservableCollection<Customer> _customers; // list customer


        // navigator chuyen den edit
        private readonly IContextualNavigatorService<EditCustomerVM, int> editCustomerNavigator;

        public CustomerVM(ApplicationDbContext db,
            IContextualNavigatorService<EditCustomerVM,int> editCustomerNavigator)
        {
            this.db = db;
            this.editCustomerNavigator = editCustomerNavigator;
        }


        // load du lieu
        public override void ResetState()
        {
            base.ResetState();
            var customers = db.Customers.ToList();
            Customers = new(customers);
        }

        // xoa
        [RelayCommand]
        private void DeleteCustomer(Customer customer)
        {
            if (customer == null) return;
            Customers.Remove(customer);
            //MessageBox.Show("Delete");
        }

        // chinh sua
        [RelayCommand]
        private void EditCustomer(Customer customer)
        {
            if (customer == null) return;
            editCustomerNavigator.Navigate(customer.Id);
        }
    }
}

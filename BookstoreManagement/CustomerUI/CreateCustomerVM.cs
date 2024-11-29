using BookstoreManagement.Core;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class CreateCustomerVM : BaseViewModel
    {
        private readonly ApplicationDbContext db;
        private readonly INavigatorService<AllCustomersVM> customerNavigator;

        [ObservableProperty]
        private string _firstName;

        [ObservableProperty]
        private string _lastName;

        [ObservableProperty]
        private string _phoneNumber;

        [ObservableProperty]
        private string _email;

        [RelayCommand]

        // Them khach hang
        private void Submit()
        {
            var customer = new Customer
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                Email = Email
            };
            try
            {
                db.Add(customer);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add employee : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Added employee successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void ResetToDefaultValues()
        {
            _firstName = string.Empty;
            _lastName = string.Empty;
            _phoneNumber = string.Empty;
            _email = string.Empty;
        }


        /// cap nhap trang thai
        public override void ResetState()
        {
            base.ResetState();
            ResetToDefaultValues();
        }

        // Nut tro ve lai
        [RelayCommand]
        private void GoBack()
        {
            customerNavigator.Navigate();
        }
        public CreateCustomerVM(ApplicationDbContext db,
            INavigatorService<AllCustomersVM> customernNavigator)
        {
            ResetToDefaultValues();
            this.db = db;
            this.customerNavigator = customernNavigator;
        }

    }
}

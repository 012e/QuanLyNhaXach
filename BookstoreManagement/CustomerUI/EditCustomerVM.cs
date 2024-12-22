using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System.Windows;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class EditCustomerVM : EditItemVM<Customer>
    {
        private readonly ApplicationDbContext db;

        [ObservableProperty]
        private Customer _customer;
        [ObservableProperty]
        private string _errorMessage =string.Empty;
        [ObservableProperty]
        private bool _isSubmitSuccess = false;
        private readonly INavigatorService<AllCustomersVM> customerNavigator;
        public EditCustomerVM(ApplicationDbContext db,
            INavigatorService<AllCustomersVM> customerNavigator)
        {
            this.db = db;
            this.customerNavigator = customerNavigator;
            Customer = new Customer();
        }
        [RelayCommand]
        private void GoBack()
        {
            customerNavigator.Navigate();
        }
        protected override void LoadItem()
        {
            Customer = default;
            var id = ViewModelContext.Id;
            Customer = db.Customers.Find(id);
        }
        protected override void SubmitItemHandler()
        {
            if (!Check_Valid_Input())
            {
                MessageBox.Show(ErrorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            db.Customers.Update(Customer);
            db.SaveChanges();
            IsSubmitSuccess = true;
        }
        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            if(IsSubmitSuccess)
            {
                MessageBox.Show("submit successfully");
                IsSubmitSuccess = false;
            }
            return;
        }
        private bool IsOnlyLetterAndSpaces(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z\s]+$");
        }
        private bool IsOnlyNumber(string input)
        {
            return Regex.IsMatch(input, @"\d");
        }
        private bool Check_Valid_Input()
        {
            if (string.IsNullOrWhiteSpace(Customer.FirstName))
            {
                ErrorMessage = "Customer first name can not be empty!";
                return false;
            }
            if (!IsOnlyLetterAndSpaces(Customer.FirstName))
            {
                ErrorMessage = "Customer first name must contain only letters and spaces!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Customer.LastName))
            {
                ErrorMessage = "Customer last name can not be empty!";
                return false;
            }
            if (!IsOnlyLetterAndSpaces(Customer.LastName))
            {
                ErrorMessage = "Customer last name must contain only letters and spaces!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Customer.PhoneNumber))
            {
                ErrorMessage = "Customer phone number can not be empty!";
                return false;
            }
            if (!IsOnlyNumber(Customer.PhoneNumber))
            {
                ErrorMessage = "Customer phone number must contain only number!";
                return false;
            }
            if (Customer.PhoneNumber.Length != 10)
            {
                ErrorMessage = "Customer phone number must have only 10 number!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Customer.Email))
            {
                ErrorMessage = "Customer email can not be empty!";
                return false;
            }
            bool validEmail = Customer.Email.Contains('@');
            if (!validEmail)
            {
                ErrorMessage = "Customer email is not a valid type ( must contain @ )!";
                return false;
            }
            ErrorMessage = string.Empty;
            return true;

        }

    }

}

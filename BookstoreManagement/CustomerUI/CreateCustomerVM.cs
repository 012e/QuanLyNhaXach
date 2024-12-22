using BookstoreManagement.Core;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System.Windows;

namespace BookstoreManagement.UI.CustomerUI
{
    public partial class CreateCustomerVM : BaseViewModel
    {
        private readonly ApplicationDbContext db;
        private readonly INavigatorService<AllCustomersVM> customerNavigator;

        [ObservableProperty]
        private Customer _customer;
        [ObservableProperty]
        private string _errorMessage = string.Empty;
        [RelayCommand]
        private void Submit()
        {
            if (!Check_Valid_Input())
            {
                MessageBox.Show(ErrorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                db.Add(Customer);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add employee : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Added employee successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if(Customer.PhoneNumber.Length != 10)
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
                return false ;
            }
            ErrorMessage = string.Empty;    
            return true;

        }

        private void ResetToDefaultValues()
        {
            Customer = new Customer()
            {
                FirstName = "",
                LastName = "",
                PhoneNumber = "",
                Email =""
            };
        }
        public override void ResetState()
        {
            base.ResetState();
            ResetToDefaultValues();
        }

        [RelayCommand]
        private void GoBack()
        {
            customerNavigator.Navigate();
        }
        public CreateCustomerVM(ApplicationDbContext db,
            INavigatorService<AllCustomersVM> customernNavigator)
        { 
            this.db = db;
            this.customerNavigator = customernNavigator;
        }

    }
}

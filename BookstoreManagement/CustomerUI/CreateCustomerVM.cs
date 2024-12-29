using BookstoreManagement.Core;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using System.Windows;
using ToastNotifications.Core;

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
            try
            {
                if (!Check_Valid_Input())
                {
                    ErrorNotification();
                    return;
                }
                db.Add(Customer);
                db.SaveChanges();
                SuccessNotification();
                customerNavigator.Navigate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add customer : Database Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
            
        private bool IsOnlyLetterAndSpaces(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z\s]+$");
        }
        private bool IsOnlyNumber(string input)
        {
            return Regex.IsMatch(input, @"\d");
        }
        private bool IsValidEmail(string input)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(input, emailPattern);
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
            if (!IsValidEmail(Customer.Email))
            {
                ErrorMessage = "Customer email is not a valid type (example@example.com)!";
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

        private void SuccessNotification()
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", "Added customer successfully", NotificationType.Error, new MessageOptions
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
}

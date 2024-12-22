using BookstoreManagement.Core;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2013.Excel;
using System.Text.RegularExpressions;
using System.Windows;

namespace BookstoreManagement.UI.EmployeeUI
{
    public partial class CreateEmployeeVM : BaseViewModel
    {
        private readonly ApplicationDbContext Db;
        private readonly INavigatorService<AllEmployeeVM> allEmployeeNavigator;

        [ObservableProperty]
        private Employee _employee = new()
        {
            ProfilePicture = ""
        };
        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public CreateEmployeeVM(ApplicationDbContext db, INavigatorService<AllEmployeeVM> allEmployeeNavigator)
        {
            Db = db;
            this.allEmployeeNavigator = allEmployeeNavigator;
        }
        [RelayCommand]
        private void NavigateBack()
        {
            allEmployeeNavigator.Navigate();
        }
        [RelayCommand]
        private void Submit()
        {
            if (!Check_Valid_Input())
            {
                MessageBox.Show(ErrorMessage , "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                Db.Add(Employee);
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add employee : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Added employee successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void ResetToDefaultValues()
        {
            Employee = new Employee
            {
                FirstName = "",
                LastName = "",
                Email = "",
                Password = "",
                ProfilePicture = "",
                Salary = 0,
                IsManager = false
            };
        }
        private bool IsOnlyLetterAndSpaces(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z\s]+$");
        }
        private bool Check_Valid_Input()
        {
            if (string.IsNullOrWhiteSpace(Employee.FirstName))
            {
                ErrorMessage = "Employee first name can not be empty!";
                return false;
            }
            if(!IsOnlyLetterAndSpaces(Employee.FirstName))
            {
                ErrorMessage = "Employee first name must contain only letters!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Employee.LastName))
            {
                ErrorMessage = "Employee last name can not be empty!";
                return false;
            }
            if (!IsOnlyLetterAndSpaces(Employee.LastName))
            {
                ErrorMessage = "Employee last name must contain only letters!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Employee.Email))
            {
                ErrorMessage = "Employee email can not be empty!";
                return false;
            }
            bool validEmail = Employee.Email.Contains('@');
            if (!validEmail)
            {
                ErrorMessage = "Employee email is not a valid type ( must contain @ )!";
                return false ;
            }
            if(Employee.Salary < 0)
            {
                ErrorMessage = "Employee salary must be a non-negative integer!";
                return false;
            }
            if (!Is_Password_Valid())
            {
                return false;
            }
            ErrorMessage = string.Empty;
            return true;
        }
        private bool Is_Password_Valid()
        {
            string password = Employee.Password;
            if(password.Length < 8)
            {
                ErrorMessage = "Password must be at least 8 characters long!";
                return false;  
            }
            if(!Regex.IsMatch(password , @"[A-Z]"))
            {
                ErrorMessage = "Password must contain at least one uppercase letter!";
                return false;
            }
            if(!Regex.IsMatch(password, @"[a-z]"))
            {
                ErrorMessage = "Password must contain at least one lowercase letter!";
                return false;
            }
            if (!Regex.IsMatch(password, @"\d"))
            {
                ErrorMessage = "Password must contain at least one digit!";
                return false;
            }
            if(!Regex.IsMatch(password , @"[!@#$%^&*()\-_=+\[\]{};:'"",.<>?/\\]"))
            {
                ErrorMessage = "Password must contain at least special character!";
                return false;
            }
            if(password.Contains(" "))
            {
                ErrorMessage = "Password must not contain any spaces!";
                return false;
            }
            return true;
        }
        public override void ResetState()
        {
            ResetToDefaultValues();
            base.ResetState();
            
        }
    }
}

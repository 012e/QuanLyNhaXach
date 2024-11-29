using BookstoreManagement.Core;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookstoreManagement.UI.EmployeeUI
{
    public partial class CreateEmployeeVM : BaseViewModel
    {
        private readonly ApplicationDbContext Db;
        private readonly INavigatorService<AllEmployeeVM> allEmployeeNavigator;

        [ObservableProperty]
        private String _firstName;
        [ObservableProperty]
        private String _lastName;
        [ObservableProperty]
        private String _email;
        [ObservableProperty]
        private decimal _salary;
        [ObservableProperty]
        private byte[] _profilePicture;
        [ObservableProperty]
        private bool _isManager;

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
            var item = new Employee
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Salary = Salary,
                ProfilePicture = "",
                IsManager = IsManager,
            };
            try
            {
                Db.Add(item);
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
            _firstName = string.Empty;
            _lastName = string.Empty;
            _email = string.Empty;
            _salary = 0;
            _isManager = false;
        }
        public override void ResetState()
        {
            base.ResetState();
            ResetToDefaultValues();
        }
    }
}

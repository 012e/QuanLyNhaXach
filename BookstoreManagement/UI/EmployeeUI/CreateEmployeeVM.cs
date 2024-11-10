using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookstoreManagement.UI.EmployeeUI;

public partial class CreateEmployeeVM : BaseViewModel
{
    private readonly ApplicationDbContext Db;
    private readonly INavigatorService<AllEmployeeVM> allEmployeeNavigator;

    [ObservableProperty]
    private string _firstName;
    [ObservableProperty] 
    private string _lastName;
    [ObservableProperty]
    private string _email;
    [ObservableProperty]
    private decimal _salary;
    [ObservableProperty]
    private bool _isManager;

    public CreateEmployeeVM(ApplicationDbContext db , INavigatorService<AllEmployeeVM> allemployeeNavigator)
    {
        Db = db;
        allEmployeeNavigator = allemployeeNavigator;
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
            IsManager = IsManager
        };
        try
        {
            Db.Add(item);
            Db.SaveChanges();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Couldn't add item! {ex}" , "Error" , MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        MessageBox.Show("Added item successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}

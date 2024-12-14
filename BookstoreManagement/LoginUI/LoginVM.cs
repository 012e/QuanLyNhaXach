using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Dtos;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.MainUI;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace BookstoreManagement.LoginUI;

public partial class LoginVM : BaseViewModel
{
    private readonly INavigatorService<MainVM> mainUINavigator;
    private readonly LoginService loginService;
    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _password;


    [RelayCommand]
    private async void Login()
    {
        var dto = new LoginDto
        {
            Email = Email,
            Password = Password
        };

        if (loginService.Login(dto) == null)
        {
            MessageBox.Show($"Login failed. Wrong email or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        mainUINavigator.Navigate("global");
    }

    public override void ResetState()
    {
        base.ResetState();
        Password = "";
        Email = "";
    }

    public LoginVM(INavigatorService<MainVM> mainUINavigator, LoginService loginService)
    {
        this.mainUINavigator = mainUINavigator;
        this.loginService = loginService;
    }
}

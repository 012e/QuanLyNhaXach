using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Dtos;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.MainUI;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace BookstoreManagement.LoginUI;

public partial class LoginVM : BaseViewModel
{
    private readonly INavigatorService<MainVM> mainUINavigator;
    private readonly LoginService loginService;
    [ObservableProperty]
    private string _email;
    [ObservableProperty]
    private string _password;
    [ObservableProperty]
    private bool _isPasswordVisible = false;
    [ObservableProperty]
    private bool _isPasswordNotVisible = true;
    [ObservableProperty]
    private PackIconKind _passwordVisibilityIcon = PackIconKind.EyeOff;


    [RelayCommand]
    private async Task Login()
    {
        var dto = new LoginDto
        {
            Email = Email,
            Password = Password
        };

        if (await loginService.LoginAsync(dto) == null)
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
    [RelayCommand]
    private void HideAndShow()
    {
        IsPasswordVisible = !IsPasswordVisible;
        IsPasswordNotVisible = !IsPasswordNotVisible;
        PasswordVisibilityIcon = IsPasswordVisible ? PackIconKind.Eye : PackIconKind.EyeOff;
    }
}

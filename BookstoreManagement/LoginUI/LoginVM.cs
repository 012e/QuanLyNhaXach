﻿using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Dtos;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.MainUI;
using BookstoreManagement.Shared.Services;
using BookstoreManagement.UI.DashboardUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flattinger.Core.Interface.ToastMessage;
using Flattinger.UI.ToastMessage;
using Flattinger.UI.ToastMessage.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace BookstoreManagement.LoginUI;

public partial class LoginVM : BaseViewModel
{
    private readonly INavigatorService<MainVM> mainUINavigator;
    private readonly LoginService loginService;
    private readonly INavigatorService<DashBoardVM> dashboardNavigator;
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
            //ToastLoginSuccess();
            MessageBox.Show($"Login failed. Wrong email or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        mainUINavigator.Navigate("global");
        dashboardNavigator.Navigate();
    }

    public override void ResetState()
    {
        base.ResetState();
        Password = "";
        Email = "";
    }

    public LoginVM(INavigatorService<MainVM> mainUINavigator, LoginService loginService, 
        INavigatorService<DashBoardVM> dashboardNavigator)
    {
        this.mainUINavigator = mainUINavigator;
        this.loginService = loginService;
        this.dashboardNavigator = dashboardNavigator;
    }

    [RelayCommand]
    private void HideAndShow()
    {
        IsPasswordVisible = !IsPasswordVisible;
        IsPasswordNotVisible = !IsPasswordNotVisible;
        PasswordVisibilityIcon = IsPasswordVisible ? PackIconKind.Eye : PackIconKind.EyeOff;
    }

    //private void ToastLoginSuccess()
    //{
    //    ToastProvider.NotificationService.AddNotification(Flattinger.Core.Enums.ToastType.SUCCESS,
    //        "Login successful",
    //        "Welcome back, you’ve successfully logged in!",3,animationConfig: new AnimationConfig { });
    //}
}

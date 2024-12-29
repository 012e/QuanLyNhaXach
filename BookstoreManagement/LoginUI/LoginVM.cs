using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Dtos;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.MainUI;
using BookstoreManagement.Shared.Services;
using BookstoreManagement.UI.DashboardUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.ComponentModel.Design.Serialization;
using ToastNotifications.Core;
using Microsoft.Extensions.Options;
using ToastNotifications.Events;
using BookstoreManagement.Shared.CustomMessages;
using ToastNotifications.Lifetime.Clear;

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
            ErrorNotification();
            return;
        }
        SuccessNotification();
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

    private void SuccessNotification()
    {
        GetNotification.NotifierInstance.SuccessMessage("Success", "Welcome back H2K BookStore !", NotificationType.Success, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
        
    }
    private void ErrorNotification()
    {
        GetNotification.NotifierInstance.ErrorMessage("Login error", "Wrong username or passrword !", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }

  




}

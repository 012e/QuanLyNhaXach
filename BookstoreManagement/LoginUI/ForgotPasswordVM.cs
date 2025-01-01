using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Security.Cryptography;
using ToastNotifications.Core;

namespace BookstoreManagement.LoginUI;

public partial class ForgotPasswordVM : BaseViewModel
{
    public ForgotPasswordVM(LoginService loginService, INavigatorService<LoginVM> loginNavigator)
    {
        this.loginService = loginService;
        this.loginNavigator = loginNavigator;
    }

    [ObservableProperty]
    private string _email;
    private readonly LoginService loginService;
    private readonly INavigatorService<LoginVM> loginNavigator;

    public override void ResetState()
    {
        base.ResetState();
        Email = "";

    }

    private void SuccessNotification(string msg)
    {
        GetNotification.NotifierInstance.SuccessMessage("Success", msg, NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }

    private void ErrorNotification(string msg)
    {
        GetNotification.NotifierInstance.ErrorMessage("Error", msg, NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }

    [RelayCommand]
    private void GoBack()
    {
        loginNavigator.Navigate("global");
    }

    [RelayCommand]
    private void ForgotPassword()
    {
        try
        {
            loginService.ForgotPasswordAsync(Email);
        }
        catch (Exception e)
        {
            ErrorNotification(e.Message);
            return;
        }

        SuccessNotification("Recovery email sent.");
        loginNavigator.Navigate("global");
    }
}

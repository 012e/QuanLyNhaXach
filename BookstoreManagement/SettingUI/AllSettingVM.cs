using BookstoreManagement.Core;
using BookstoreManagement.LoginUI;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications.Core;

namespace BookstoreManagement.SettingUI
{
    public partial class AllSettingVM : BaseViewModel
    {
        private readonly INavigatorService<MyProfileVM> myProfileNavigator;
        private readonly INavigatorService<AccountVM> accountNavigator;
        private readonly INavigatorService<AllNotificationVM> allNotificationNavigator;
        private readonly INavigatorService<LoginVM> loginNavigator;
        [ObservableProperty]
        private NavigatorStore _navigatorStore;
        public AllSettingVM([FromKeyedServices("setting")] NavigatorStore navigatorStore,
            INavigatorService<MyProfileVM> myProfileNavigator,
            INavigatorService<AccountVM> accountNavigator,
            INavigatorService<AllNotificationVM> allNotificationNavigator,
            INavigatorService<LoginVM> loginNavigator)
        {
            this.NavigatorStore = navigatorStore;
            this.myProfileNavigator = myProfileNavigator;
            this.accountNavigator = accountNavigator;
            this.allNotificationNavigator = allNotificationNavigator;
            this.loginNavigator = loginNavigator;
        }

        [RelayCommand]
        private void NavigateToMyProfile()
        {
            myProfileNavigator.Navigate("setting");
        }

        [RelayCommand]
        private void NavigateToAccount()
        {
            accountNavigator.Navigate("setting");
        }

        private void SuccessNotification(string msg)
        {
            GetNotification.NotifierInstance.SuccessMessage("Success", msg, NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }

        private void WarningNotification()
        {
            GetNotification.NotifierInstance.WarningMessage("Warning", "This action cannot be undone", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
        }


        [RelayCommand]
        private void Logout()
        {
            WarningNotification();
            MessageBoxResult result = MessageBox.Show("Are you sure you want to logout?", "Log Out", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            loginNavigator.Navigate("global");
            SuccessNotification("Logged out successfully");
        }

        [RelayCommand]
        private void NavigateToNotification()
        {
            allNotificationNavigator.Navigate("setting");
        }
    }
}

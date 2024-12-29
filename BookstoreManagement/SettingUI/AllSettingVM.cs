using BookstoreManagement.Core;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.SettingUI
{
    public partial class AllSettingVM : BaseViewModel
    {
        private readonly INavigatorService<MyProfileVM> myProfileNavigator;
        private readonly INavigatorService<AccountVM> accountNavigator;
        private readonly INavigatorService<AllNotificationVM> allNotificationNavigator;
        [ObservableProperty]
        private NavigatorStore _navigatorStore;
        public AllSettingVM([FromKeyedServices("setting")] NavigatorStore navigatorStore,
            INavigatorService<MyProfileVM> myProfileNavigator,
            INavigatorService<AccountVM> accountNavigator,
            INavigatorService<AllNotificationVM> allNotificationNavigator)
        {
            this.NavigatorStore = navigatorStore;
            this.myProfileNavigator = myProfileNavigator;
            this.accountNavigator = accountNavigator;
            this.allNotificationNavigator = allNotificationNavigator;
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

        [RelayCommand]
        private void NavigateToNotification()
        {
            allNotificationNavigator.Navigate("setting");
        }
    }
}

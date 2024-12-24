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
        [ObservableProperty]
        private NavigatorStore _navigatorStore;
        public AllSettingVM([FromKeyedServices("setting")] NavigatorStore navigatorStore,
            INavigatorService<MyProfileVM> myProfileNavigator)
        {
            this.NavigatorStore = navigatorStore;
            this.myProfileNavigator = myProfileNavigator;
        }

        [RelayCommand]
        private void NavigateToMyProfile()
        {
            myProfileNavigator.Navigate("setting");
        }
    }
}

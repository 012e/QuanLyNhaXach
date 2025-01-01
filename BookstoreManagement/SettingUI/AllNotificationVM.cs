using BookstoreManagement.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using DocumentFormat.OpenXml.Drawing;

namespace BookstoreManagement.SettingUI
{
    public partial class AllNotificationVM : BaseViewModel
    {
        [ObservableProperty]
        private GridLength _widthDetailNote = new GridLength(0,GridUnitType.Star);

        [ObservableProperty]
        private GridLength _widthListNote = new GridLength(1,GridUnitType.Star);

        [RelayCommand]
        private void Test()
        {
            WidthDetailNote = new GridLength(0.8, GridUnitType.Star);
            WidthListNote = new GridLength(1, GridUnitType.Star);
        }
        public AllNotificationVM()
        {
            
        }
    }
}

using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.ImportUI;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using BookstoreManagement.UI.CustomerUI;
using BookstoreManagement.UI.DashboardUI;
using BookstoreManagement.UI.EmployeeUI;
using BookstoreManagement.UI.InvoicesUI;
using BookstoreManagement.UI.ItemUI;
using BookstoreManagement.UI.ProviderUI;
using BookstoreManagement.UI.TagUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ToastNotifications.Core;

namespace BookstoreManagement.UI.MainWindow;

public partial class MainWindowVM : BaseViewModel
{
    [ObservableProperty]
    private NavigatorStore _navigatorStore;

    public MainWindowVM(
        [FromKeyedServices("global")] NavigatorStore navigatorStore
    )
    {
        this.NavigatorStore = navigatorStore;
    }
    [RelayCommand]
    private void CloseApp()
    {
        GetNotification.NotifierInstance.ErrorMessage("Warning", "This action can be undone!", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
        MessageBoxResult result = MessageBox.Show(
             "Are you sure you want to exit?",
                "Confirm Exit",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
    }
}

using BookstoreManagement.Core;
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
}

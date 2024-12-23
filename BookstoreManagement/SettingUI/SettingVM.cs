using BookstoreManagement.Core;
using BookstoreManagement.Shared.Services;
using BookstoreManagement.UI.DashboardUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace BookstoreManagement.SettingUI;

public partial class SettingVM : BaseViewModel
{
    [ObservableProperty]
    private NavigatorStore _navigatorStore;

    private readonly INavigatorService<AVM> aNav;
    private readonly INavigatorService<BVM> bNav;
    private readonly INavigatorService<DashBoardVM> dashboardNavigator;

    // nho la phai giong cai hoi nay nha
    public SettingVM([FromKeyedServices("shit")] NavigatorStore navigatorStore,
        INavigatorService<AVM> aNav,
        INavigatorService<BVM> bNav,
        INavigatorService<DashBoardVM> dashboardNavigator)
    {
        this.NavigatorStore = navigatorStore;
        this.aNav = aNav;
        this.bNav = bNav;
        this.dashboardNavigator = dashboardNavigator;
    }

    [RelayCommand]
    private void A()
    {
        aNav.Navigate("shit");
    }

    [RelayCommand]
    private void B()
    {
        bNav.Navigate("shit");
    }

    [RelayCommand]
    private void NavDashboard()
    {
        dashboardNavigator.Navigate("shit");
    }
}

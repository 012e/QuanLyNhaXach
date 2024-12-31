using BookstoreManagement.Core;
using BookstoreManagement.ImportUI;
using BookstoreManagement.PricingUI;
using BookstoreManagement.LoginUI;
using BookstoreManagement.SettingUI;
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

namespace BookstoreManagement.MainUI;

public partial class MainVM : BaseViewModel
{
    [ObservableProperty]
    private NavigatorStore _navigatorStore;

    private readonly INavigatorService<AllItemsVM> itemNavigator;
    private readonly INavigatorService<AllInvoicesVM> invoiceNavigator;
    private readonly INavigatorService<AllTagsVM> tagNavigator;
    private readonly INavigatorService<DashBoardVM> dashboardNavigator;
    private readonly INavigatorService<AllEmployeeVM> employeeNavigator;
    private readonly INavigatorService<AllProviderVM> providerNavigator;
    private readonly INavigatorService<AllCustomersVM> customerNavigator;
    private readonly INavigatorService<AllImportVM> importNavigator;
    private readonly INavigatorService<AllPricingVM> pricingNavigator;
    private readonly INavigatorService<AllSettingVM> settingNavigator;
    private readonly INavigatorService<MyProfileVM> profileNavigator;
    [ObservableProperty]
    private String _title = "Bookstore Management";

    [RelayCommand]
    private void NavigateToItem()
    {
        itemNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToInvoice()
    {
        invoiceNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToTag()
    {
        tagNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToDashBoard()
    {
        dashboardNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToEmployee()
    {
        employeeNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToProvider()
    {
        providerNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToCustomer()
    {
        customerNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToImport()
    {
        importNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToPricing()
    {
        pricingNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToSetting()
    {
        profileNavigator.Navigate("setting");
        settingNavigator.Navigate();
    }

    public MainVM(
        [FromKeyedServices("default")] NavigatorStore navigatorStore,
        INavigatorService<AllItemsVM> itemNavigator,
        INavigatorService<AllInvoicesVM> invoiceNavigator,
        INavigatorService<AllTagsVM> tagNavigator,
        INavigatorService<DashBoardVM> dashBoardNavigator,
        INavigatorService<AllProviderVM> providerNavigator,
        INavigatorService<AllEmployeeVM> employeeNavigator,
        INavigatorService<AllCustomersVM> customerNavigator,
        INavigatorService<AllImportVM>  importNavigator,
        INavigatorService<AllPricingVM>  pricingNavigator,
        INavigatorService<AllSettingVM> settingNavigator,
        INavigatorService<MyProfileVM> profileNavigator
    )
    {
        this.NavigatorStore = navigatorStore;
        this.itemNavigator = itemNavigator;
        this.invoiceNavigator = invoiceNavigator;
        this.tagNavigator = tagNavigator;
        this.dashboardNavigator = dashBoardNavigator;
        this.providerNavigator = providerNavigator;
        this.employeeNavigator = employeeNavigator;
        this.customerNavigator = customerNavigator;
        this.importNavigator = importNavigator;
        this.pricingNavigator = pricingNavigator;
        this.settingNavigator = settingNavigator;
        this.profileNavigator = profileNavigator;
    }
}

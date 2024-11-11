using BookstoreManagement.UI.ItemUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreManagement.Core;
using BookstoreManagement.Services;
using BookstoreManagement.UI.InvoicesUI;
using BookstoreManagement.UI.TagUI;
using BookstoreManagement.UI.DashboardUI;
using BookstoreManagement.UI.CustomerUI;
using BookstoreManagement.UI.PromotionUI;

namespace BookstoreManagement.UI.MainWindow;

public partial class MainWindowVM : BaseViewModel
{
    [ObservableProperty]
    private NavigatorStore _navigatorStore;

    private readonly INavigatorService<AllItemsVM> itemNavigator;
    private readonly INavigatorService<AllInvoicesVM> invoiceNavigator;
    private readonly INavigatorService<AllTagsVM> tagNaviator;
    private readonly INavigatorService<DashBoardVM> dashboardNavigator;
    private readonly INavigatorService<CustomerVM> customerNavigator;
    private readonly INavigatorService<PromotionVM> promotionNavigator;


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
        tagNaviator.Navigate();
    }
    [RelayCommand]
    private void NavigateToDashBoard()
    {
        dashboardNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToCustomer()
    {
        customerNavigator.Navigate();
    }

    [RelayCommand]
    private void NavigateToPromotion()
    {
        promotionNavigator.Navigate();
    }

    public MainWindowVM(
        NavigatorStore navigatorStore,
        INavigatorService<AllItemsVM> itemNavigator,
        INavigatorService<AllInvoicesVM> invoiceNavigator,
        INavigatorService<AllTagsVM> tagNaviator,
        INavigatorService<DashBoardVM> dashBoardNavigator,
        INavigatorService<CustomerVM> customerNavigator,
        INavigatorService<PromotionVM> promotionNavigator
    )
    {
        this._navigatorStore = navigatorStore;
        this.itemNavigator = itemNavigator;
        this.invoiceNavigator = invoiceNavigator;
        this.tagNaviator = tagNaviator;
        this.dashboardNavigator = dashBoardNavigator;
        this.customerNavigator = customerNavigator;
        this.promotionNavigator = promotionNavigator;
    }
}

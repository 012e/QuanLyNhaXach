using BookstoreManagement.Core;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.UI.ItemUI;

public partial class CreateItemVM : BaseViewModel
{
    private readonly INavigatorService<AllItemsVM> allItemsNavigator;

    [RelayCommand]
    private void NavigateBack()
    {
        allItemsNavigator.Navigate();
    }

    public CreateItemVM(INavigatorService<AllItemsVM> allItemsNavigator)
    {
        this.allItemsNavigator = allItemsNavigator;
    }
}

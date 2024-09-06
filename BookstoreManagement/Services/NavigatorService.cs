using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreManagement.Core;
using BookstoreManagement.Core.Interface;

namespace BookstoreManagement.Services;



public partial class NavigatorService<TViewModel> : INavigatorService<TViewModel>
    where TViewModel : BaseViewModel
{
    public NavigatorService(NavigatorStore navigatorStore, IAbstractFactory<TViewModel> viewModelFactory)
    {
        NavigatorStore = navigatorStore;
        ViewModelFactory = viewModelFactory;
    }

    public NavigatorStore NavigatorStore { get; }

    public IAbstractFactory<TViewModel> ViewModelFactory { get; }

    public void Navigate()
    {
        var viewModel = ViewModelFactory.Create();
        viewModel.ResetState();
        NavigatorStore.CurrentViewModel = viewModel;
    }
}
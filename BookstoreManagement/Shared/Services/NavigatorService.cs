using BookstoreManagement.Core;
using BookstoreManagement.Core.Interface;

namespace BookstoreManagement.Shared.Services;



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
        NavigatorStore.CurrentViewModel = viewModel;
    }
}
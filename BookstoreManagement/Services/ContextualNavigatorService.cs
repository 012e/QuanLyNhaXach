using BookstoreManagement.Core;
using BookstoreManagement.Core.Interface;

namespace BookstoreManagement.Services;

public class ContextualNavigatorService<TViewModel, Context> : IContextualNavigatorService<TViewModel, Context>
    where TViewModel : ContextualViewModel<Context>
{
    private NavigatorStore NavigatorStore { get; }
    private IAbstractFactory<TViewModel> ViewModelFactory { get; }

    public ContextualNavigatorService(NavigatorStore navigatorStore, IAbstractFactory<TViewModel> viewModelFactory)
    {
        NavigatorStore = navigatorStore;
        ViewModelFactory = viewModelFactory;
    }

    public void Navigate(Context context)
    {
        var viewModel = ViewModelFactory.Create();
        viewModel.ResetState();
        if (context is null)
        {
            throw new ArgumentException("context can't be null");
        }
        viewModel.ViewModelContext = context;
        NavigatorStore.CurrentViewModel = viewModel;
        viewModel.OnSwitch();
    }
}
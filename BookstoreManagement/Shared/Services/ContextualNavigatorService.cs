using BookstoreManagement.Core;
using BookstoreManagement.Core.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BookstoreManagement.Shared.Services;

public class ContextualNavigatorService<TViewModel, Context> : IContextualNavigatorService<TViewModel, Context>
    where TViewModel : BaseViewModel, IContextualViewModel<Context>
{
    private readonly IServiceProvider serviceProvider;

    private IAbstractFactory<TViewModel> ViewModelFactory { get; }

    public ContextualNavigatorService(IAbstractFactory<TViewModel> viewModelFactory, IServiceProvider serviceProvider)
    {
        ViewModelFactory = viewModelFactory;
        this.serviceProvider = serviceProvider;
    }

    public void Navigate(Context context, string? @namespace = "default")
    {
        var viewModel = ViewModelFactory.Create();
        if (context is null)
        {
            throw new ArgumentException("context can't be null");
        }
        var store = serviceProvider.GetRequiredKeyedService<NavigatorStore>(@namespace);
        viewModel.ViewModelContext = context;
        store.CurrentViewModel = viewModel;
        viewModel.SetupContext();
    }
}
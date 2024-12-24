using BookstoreManagement.Core;
using BookstoreManagement.Core.Interface;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;

namespace BookstoreManagement.Shared.Services;

public partial class NavigatorService<TViewModel> : INavigatorService<TViewModel>
    where TViewModel : BaseViewModel
{
    private readonly IServiceProvider serviceProvider;

    public NavigatorService(IAbstractFactory<TViewModel> viewModelFactory, IServiceProvider serviceProvider)
    {
        ViewModelFactory = viewModelFactory;
        this.serviceProvider = serviceProvider;
    }

    public NavigatorStore NavigatorStore { get; }

    public IAbstractFactory<TViewModel> ViewModelFactory { get; }

    public void Navigate(string? @namespace = "default")
    {
        var viewModel = ViewModelFactory.Create();
        var store = this.serviceProvider.GetRequiredKeyedService<NavigatorStore>(@namespace);
        store.CurrentViewModel?.CleanUp();
        store.CurrentViewModel = viewModel;
        store.CurrentViewModel.ResetState();
    }
}
using BookstoreManagement.Core;

namespace BookstoreManagement.Shared.Services;

public interface IContextualNavigatorService<TViewModel, Context>
    where TViewModel : BaseViewModel, IContextualViewModel<Context>
{
    void Navigate(Context context, string? @namespace = "default");
}
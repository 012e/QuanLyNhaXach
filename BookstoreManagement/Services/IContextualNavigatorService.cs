using BookstoreManagement.Core;

namespace BookstoreManagement.Services;

public interface IContextualNavigatorService<TViewModel, Context>
    where TViewModel : BaseViewModel, IContextualViewModel<Context>
{
    void Navigate(Context context);
}
using BookstoreManagement.Core;

namespace BookstoreManagement.Services;

public interface IContextualNavigatorService<TViewModel, Context> where TViewModel : ContextualViewModel<Context>
{
    void Navigate(Context context);
}
using BookstoreManagement.Core;

namespace BookstoreManagement.Services;

public interface INavigatorService<T> where T : BaseViewModel
{
    public void Navigate();
}
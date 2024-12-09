using BookstoreManagement.Core;

namespace BookstoreManagement.Shared.Services;

public interface INavigatorService<T> where T : BaseViewModel
{
    public void Navigate(string? @namespace = "default");
}
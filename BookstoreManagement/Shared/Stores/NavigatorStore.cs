using BookstoreManagement.Core;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class NavigatorStore : ObservableObject
{
    [ObservableProperty]
    private BaseViewModel _currentViewModel;

    partial void OnCurrentViewModelChanging(BaseViewModel? oldValue, BaseViewModel newValue)
    {
        if (oldValue is not null) oldValue.CleanUp();
        newValue.ResetState();
    }
}

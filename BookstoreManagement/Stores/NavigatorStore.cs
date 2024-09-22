using CommunityToolkit.Mvvm.ComponentModel;
using BookstoreManagement.Core;

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

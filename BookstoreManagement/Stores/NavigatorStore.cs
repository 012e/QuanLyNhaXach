using CommunityToolkit.Mvvm.ComponentModel;
using BookstoreManagement.Core;

public partial class NavigatorStore : ObservableObject
{
    [ObservableProperty]
    private BaseViewModel _currentViewModel;
}

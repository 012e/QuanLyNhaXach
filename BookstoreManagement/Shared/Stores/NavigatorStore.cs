using BookstoreManagement.Core;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class NavigatorStore : ObservableObject
{
    [ObservableProperty]
    private BaseViewModel _currentViewModel;
}

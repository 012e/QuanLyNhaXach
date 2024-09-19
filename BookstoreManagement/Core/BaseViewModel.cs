using CommunityToolkit.Mvvm.ComponentModel;

namespace BookstoreManagement.Core;
public abstract class BaseViewModel : ObservableObject
{
    // Happen after switched to this view model
    public virtual void ResetState()
    {
    }

    // Happen before switching to another view model
    public virtual void CleanUp()
    {

    }
}

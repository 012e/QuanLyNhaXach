using CommunityToolkit.Mvvm.ComponentModel;

namespace BookstoreManagement.Core;
public abstract class BaseViewModel : ObservableObject
{

    /// <summary>
    /// Happen after switched to this view model
    /// </summary>
    public virtual void ResetState()
    {
    }

    /// <summary>
    /// Happen before switching to another view model
    /// </summary>
    public virtual void CleanUp()
    {
    }
}

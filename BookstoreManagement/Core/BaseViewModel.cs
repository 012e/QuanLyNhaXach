using CommunityToolkit.Mvvm.ComponentModel;

namespace BookstoreManagement.Core;
public abstract class BaseViewModel : ObservableObject
{
    public virtual void ResetState()
    {
    }
}

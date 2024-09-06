using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.Core;

public abstract class ContextualViewModel<TContext> : BaseViewModel
{
    public abstract TContext? ViewModelContext { get; set; }
    public virtual void OnSwitch()
    {

    }
} 

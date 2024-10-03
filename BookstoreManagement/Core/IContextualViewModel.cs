using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.Core;

public interface IContextualViewModel<TContext>
{
    public TContext ViewModelContext { get; set; }
    /// <example>
    /// For example if TContext is int and used as the primary key for a table,
    /// and you need to get the data from ef core, you can do the following
    /// <code>
    /// public override void SetupContext()
    /// {
    ///     var id = ViewModelContext!;
    ///     TargetItem = db.DataSet.Find(id);
    /// }
    /// </code>
    /// </example>
    public virtual void SetupContext() { }
}

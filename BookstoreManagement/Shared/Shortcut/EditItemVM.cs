using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Data.Common;
using System.Windows;

namespace BookstoreManagement.Core.Shortcut;

/// <summary>
/// A base view model class for editing items that provides context management and handles loading and submitting actions asynchronously.
/// </summary>
/// <typeparam name="TItem">The type of the item being edited.</typeparam>
/// <remarks>
/// Exposes the following properties:
/// <list type="bullet">
///   <item><description>bool IsLoading — Indicates whether data is currently being loaded.</description></item>
///   <item><description>bool IsSubmitting — Indicates whether an item is currently being submitted.</description></item>
///   <item><description>TItem ViewModelContext — The current context of the view model, representing the item being edited.</description></item>
/// </list>
///
/// Implements the following methods and commands:
/// <list type="bullet">
///   <item><description>void ResetState() — Resets the loading state and clears the DbContext change tracker.</description></item>
///   <item><description>bool CanSubmitItem — Determines whether the item can be submitted (based on loading and submitting states).</description></item>
///   <item><description>void SubmitItem() — Initiates the item submission process asynchronously.</description></item>
///   <item><description>void SetupContext() — Loads data in the background by invoking LoadDataInBackground().</description></item>
/// </list>
///
/// Provides the following abstract and virtual methods for customization:
/// <list type="bullet">
///   <item><description>abstract DbContext Db — The EF Core DbContext used for database operations.</description></item>
///   <item><description>abstract void LoadItem() — Method to load the item, to be implemented by derived classes.</description></item>
///   <item><description>abstract void SubmitItemHandler() — Method to handle item submission, to be implemented by derived classes.</description></item>
///   <item><description>virtual void HandleLoadingException(Exception e) — Handles exceptions that occur during loading.</description></item>
///   <item><description>virtual void HandleSubmittingException(Exception e) — Handles exceptions that occur during submission.</description></item>
///   <item><description>virtual void OnLoadingSuccess() — Called when loading completes successfully.</description></item>
///   <item><description>virtual void OnSubmittingSuccess() — Called when submission completes successfully.</description></item>
/// </list>
///
/// This class uses asynchronous background workers for loading and submitting operations.
/// </remarks>

public abstract partial class EditItemVM<TItem> : BaseViewModel, IContextualViewModel<TItem>
    where TItem : class
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitItemCommand))]
    private bool _isLoading = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitItemCommand))]
    private bool _isSubmitting = false;

    public override void ResetState()
    {
        IsLoading = false;
        base.ResetState();
    }


    protected virtual bool CanSubmitItem => !IsLoading && !IsSubmitting;

    public TItem ViewModelContext { get; set; }

    [RelayCommand(CanExecute = nameof(CanSubmitItem))]
    protected void SubmitItem()
    {
        SubmitItemInBackground();
    }

    protected abstract void LoadItem();
    protected abstract void SubmitItemHandler();

    private void BeginLoading()
    {
        IsLoading = true;
    }

    private void FinishLoading()
    {
        IsLoading = false;
    }

    private void BeginSubmitting()
    {
        IsSubmitting = true;
    }

    private void FinishSubmitting()
    {
        IsSubmitting = false;
    }

    protected virtual void HandleLoadingException(Exception e)
    {
        if (e is DbException)
        {
            MessageBox.Show("Failed to connect to database, please check your connection.");
        }
        else
        {
            MessageBox.Show($"Some error occured, couldn't fetch data. Please refresh later.");
        }
    }

    protected virtual void HandleSubmittingException(Exception e)
    {
        if (e is DbException)
        {
            MessageBox.Show("Failed to connect to database, please check your connection.");
        }
        else
        {
            MessageBox.Show($"Some error occured, couldn't fetch data. Please refresh later.");
        }
    }

    protected virtual void OnLoadingSuccess()
    {
    }

    protected virtual void OnSubmittingSuccess()
    {
    }

    private void SubmitItemInBackground()
    {
        // let 'em load my bruh
        if (IsSubmitting) return;

        BeginSubmitting();
        BackgroundWorker worker = new();

        worker.DoWork += (send, e) =>
        {
            SubmitItemHandler();
        };

        worker.RunWorkerCompleted += (send, e) =>
        {
            FinishSubmitting();
            if (e.Error is not null)
            {
                HandleSubmittingException(e.Error);
            }
            else
            {
                OnSubmittingSuccess();
            }
        };

        worker.RunWorkerAsync();

    }

    protected void LoadDataInBackground()
    {
        // let 'em load my bruh
        if (IsLoading) return;

        BeginLoading();
        BackgroundWorker worker = new();

        worker.DoWork += (send, e) =>
        {
            LoadItem();
        };

        worker.RunWorkerCompleted += (send, e) =>
        {
            FinishLoading();
            if (e.Error is not null)
            {
                HandleLoadingException(e.Error);
            }
            else
            {
                OnLoadingSuccess();
            }
        };

        worker.RunWorkerAsync();
    }

    public void SetupContext()
    {
        LoadDataInBackground();
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data.Common;
using System.Windows;

namespace BookstoreManagement.Core.Shortcut;

public abstract partial class EditItemVM<TItem> : BaseViewModel, IContextualViewModel<TItem>
{
    // Your ef core DbContext
    protected abstract DbContext Db { get; }
    public TItem ViewModelContext { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitItemCommand))]
    private bool _isLoading = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitItemCommand))]
    private bool _isSubmitting = false;

    public override void ResetState()
    {
        IsLoading = false;
        Db.ChangeTracker.Clear();
        base.ResetState();
    }


    protected virtual bool CanSubmitItem => !IsLoading && !IsSubmitting;
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

    private void LoadDataInBackground()
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

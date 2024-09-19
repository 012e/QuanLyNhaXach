using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.Services;

namespace BookstoreManagement.UI.ItemUI;

public partial class EditItemVM : ContextualViewModel<Item>
{
    public override Item? ViewModelContext { get; set; }

    [ObservableProperty]
    private Item _item;

    [ObservableProperty]
    private Boolean _canSubmit = true;

    [ObservableProperty]
    private String _submitButtonText = "Submit";

    private void enterSubmitting()
    {
        CanSubmit = false;
        SubmitButtonText = "Submitting...";
    }

    private void finishSubmitting()
    {
        CanSubmit = true;
        SubmitButtonText = "Submit";
    }

    private readonly ApplicationDbContext db;
    private readonly INavigatorService<AllItemsVM> allItemsNavigator;

    // BUG: submit will run successfully even when data binding failed
    // possible fixes: write custom converters or validate data
    [RelayCommand]
    private async Task Submit()
    {
        enterSubmitting();
        try
        {
            db.Update(Item);
            await db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Failed to save {e}");
            return;
        }
        finally
        {
            finishSubmitting();
        }

        MessageBox.Show("Saved successfully");
    }

    [RelayCommand]
    private void NavigateBack()
    {
        allItemsNavigator.Navigate();
    }

    public override void OnSwitch()
    {
        Item = ViewModelContext;
        base.OnSwitch();
    }

    public EditItemVM(ApplicationDbContext db, INavigatorService<AllItemsVM> allItemsNavigator)
    {
        this.db = db;
        this.allItemsNavigator = allItemsNavigator;
    }
}

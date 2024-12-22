using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System.Windows;

namespace BookstoreManagement.UI.ProviderUI;

public partial class EditProviderVM : EditItemVM<Provider>
{
    private readonly ApplicationDbContext db;

    public INavigatorService<AllProviderVM> AllProvidersNavigator { get; set; }

    [ObservableProperty]
    private Provider _provider;
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    [ObservableProperty]
    private bool _isSubmitSuccess = false;
    public EditProviderVM(ApplicationDbContext db, INavigatorService<AllProviderVM> allProvidersNavigator)
    {
        this.db = db;
        AllProvidersNavigator = allProvidersNavigator;
        Provider = new Provider();
    }
    [RelayCommand]
    private void NavigateBack()
    {
        AllProvidersNavigator.Navigate();
    }
    protected override void LoadItem()
    {
        var id = ViewModelContext.Id;
       Provider = db.Providers.Find(id);
    }

    protected override void SubmitItemHandler()
    {
        if (!Check_Valid_Input())
        {
            MessageBox.Show(ErrorMessage , "Warning" , MessageBoxButton.OK , MessageBoxImage.Warning);
            return;
        }
        db.Providers.Update(Provider);
        db.SaveChanges();
        IsSubmitSuccess = true;
    }
    protected override void OnSubmittingSuccess()
    {
        base.OnSubmittingSuccess();
        if (IsSubmitSuccess)
        {
            MessageBox.Show("Submit successfully!");
            IsSubmitSuccess = true;
        }
        return;  
    }
    private bool IsOnlyLetterAndSpaces(string input)
    {
        return Regex.IsMatch(input, @"^[a-zA-Z\s,./\-#'sáàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ]+$");
    }
    private bool IsValidAddress(string input)
    {
        return Regex.IsMatch(input, @"^[a-zA-Z0-9\s,./\-#'sáàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ]+$");
    }
    private bool Check_Valid_Input()
    {
        if (string.IsNullOrWhiteSpace(Provider.Name))
        {
            ErrorMessage = "Provider name can not be empty!";
            return false;
        }
        if (!IsOnlyLetterAndSpaces(Provider.Name))
        {
            ErrorMessage = "Provider name must contain only letters and spaces!";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Provider.Address))
        {
            ErrorMessage = "Provider address can not be empty!";
            return false;
        }
        if (!IsValidAddress(Provider.Address))
        {
            ErrorMessage = "Provider address is not a valid type!";
            return false;
        }
        ErrorMessage = string.Empty;
        return true;
    }
}

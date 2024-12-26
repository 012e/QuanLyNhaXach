using BookstoreManagement.Core;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows;

namespace BookstoreManagement.UI.ProviderUI
{
    public partial class CreateProviderVM : BaseViewModel
    {
        private readonly ApplicationDbContext Db;
        private readonly INavigatorService<AllProviderVM> allProviderNavigator;

        [ObservableProperty]
        private Provider _provider;
        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public CreateProviderVM(ApplicationDbContext db, INavigatorService<AllProviderVM> AllProviderNavigator)
        {
            Db = db;
            allProviderNavigator = AllProviderNavigator;
        }
        [RelayCommand]
        private void NavigateBack()
        {
            allProviderNavigator.Navigate();
        }
        [RelayCommand]
        private void Submit()
        {
            try
            {
                if (!Check_Valid_Input())
                {
                    MessageBox.Show(ErrorMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Db.Add(Provider);
                Db.SaveChanges();
                MessageBox.Show("Added provider successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                allProviderNavigator.Navigate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add provider : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }
        private void ResetToDefaultValues()
        {
            Provider = new Provider()
            {
                Name = "",
                Address = ""
            };
        }
        private bool IsOnlyLetterAndSpaces(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z\ssáàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ]+$");
        }
     
        private bool IsValidAddress(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z0-9\s,./\-#'sáàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ]+$");
        }
        private bool Check_Valid_Input()
        {
            if(string.IsNullOrWhiteSpace(Provider.Name))
            {
                ErrorMessage = "Provider name can not be empty!";
                return false;
            }
            if(!IsOnlyLetterAndSpaces(Provider.Name))
            {
                ErrorMessage = "Provider name must contain only letters and spaces!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Provider.Address))
            {
                ErrorMessage = "Provider address can not be empty!";
                return false;
            }
            if(!IsValidAddress(Provider.Address))
            {
                ErrorMessage = "Provider address is not a valid type!";
                return false ;
            }
            ErrorMessage = string.Empty;
            return true;
        }
        public override void ResetState()
        {
            base.ResetState();
            ResetToDefaultValues();
        }
    }
}

using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace BookstoreManagement.UI.TagUI
{
    public partial class EditTagVM : EditItemVM<Tag>
    {
        private readonly ApplicationDbContext db;

        [ObservableProperty]
        private Tag _tag;
        private readonly INavigatorService<AllTagsVM> allTagsNavigator;

        [RelayCommand]
        private void NavigateBack()
        {
            allTagsNavigator.Navigate();
        }

        protected override void LoadItem()
        {
            Tag = db.Tags.Where(tag => tag.Id == ViewModelContext.Id).First();
        }

        protected override void SubmitItemHandler()
        {
            db.Tags.Update(Tag);
            db.SaveChanges();
        }

        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            MessageBox.Show("Submitted successfully");
        }

        public EditTagVM(ApplicationDbContext db, INavigatorService<AllTagsVM> allTagsNavigator)
        {
            this.db = db;
            this.allTagsNavigator = allTagsNavigator;
        }
    }
}

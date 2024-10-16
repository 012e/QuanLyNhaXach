using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookstoreManagement.UI.TagUI
{
    public partial class EditTagVM : EditItemVM<Tag>
    {
        protected override ApplicationDbContext Db { get; }

        [ObservableProperty]
        private Tag _tag;
        private readonly INavigatorService<AllTagsVM> allTagsNavigator;

        [RelayCommand]
        private void GoBack()
        {
            allTagsNavigator.Navigate();
        }

        protected override void LoadItem()
        {
            Tag = Db.Tags.Where(tag => tag.Id == ViewModelContext.Id).First();
        }

        protected override void SubmitItemHandler()
        {
            Db.Tags.Update(Tag);
            Db.SaveChanges();
        }

        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            MessageBox.Show("Submitted successfully");
        }

        public EditTagVM(ApplicationDbContext db, INavigatorService<AllTagsVM> allTagsNavigator)
        {
            Db = db;
            this.allTagsNavigator = allTagsNavigator;
        }
    }
}

using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using BookstoreManagement.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManagement.UI.TagUI
{
    public partial class AllTagsVM : ListVM<Tag, EditTagVM>
    {
        private readonly INavigatorService<CreateTagVM> createItemNavigator;

        protected override IContextualNavigatorService<EditTagVM, Tag> EditItemNavigator { get; }

        protected override ApplicationDbContext Db { get;  }

        [RelayCommand]
        private void NavigateToCreateItem()
        {
            createItemNavigator.Navigate();
        }

        public AllTagsVM(
            ApplicationDbContext db,
            IContextualNavigatorService<EditTagVM, Tag> editItemNavigator,
            INavigatorService<CreateTagVM> createItemNavigator
            )
        {
            Db = db;
            EditItemNavigator = editItemNavigator;
            this.createItemNavigator = createItemNavigator;
        }
    }
}

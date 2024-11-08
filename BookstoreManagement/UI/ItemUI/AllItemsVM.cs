﻿using BookstoreManagement.DbContexts;
using BookstoreManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using BookstoreManagement.Core;
using BookstoreManagement.Services;
using BookstoreManagement.Core.Shortcut;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using System.Windows.Data;

namespace BookstoreManagement.UI.ItemUI;

public partial class AllItemsVM : ListVM<Item, EditItemVM>
{
    protected override ApplicationDbContext Db { get; }
    protected override IContextualNavigatorService<EditItemVM, Item> EditItemNavigator { get; }
    protected INavigatorService<CreateItemVM> CreateItemNavigator { get; }

    private readonly ApplicationDbContext db;

    [ObservableProperty]
    private ObservableCollection<Item> _listItems;

    [RelayCommand]
    protected void NavigateToCreateItem()
    {
        CreateItemNavigator.Navigate();
    }

    public AllItemsVM(ApplicationDbContext db,
        IContextualNavigatorService<EditItemVM, Item> editItemNavigator,
        INavigatorService<CreateItemVM> createItemNavigator
    )
    {
        this.Db = db;
        this.EditItemNavigator = editItemNavigator;
        this.CreateItemNavigator = createItemNavigator;


        _listItems = new ObservableCollection<Item>();
        FilteredItems = CollectionViewSource.GetDefaultView(_listItems);
        FilteredItems.Filter = FilteredID;

        //this.db = db;

    }

    [ObservableProperty]
    private string _searchID;

    public ICollectionView FilteredItems { get; }



    public bool FilteredID(object item)
    {
        if(item is Item data)
        {
            if (string.IsNullOrEmpty(SearchID)) return true;
            if (int.TryParse(SearchID, out int search)) return data.Id == search;
        }
        return false;
    }

    //private void LoadData()
    //{
    //    ListItems.Clear(); // Clear the existing collection
        
    //    var items = this.Db.Items.ToList();
    //    foreach (var item in items)
    //    {
    //        ListItems.Add(item);
    //    }

    //}
    //public override void ResetState()
    //{

    //    base.ResetState();
    //    LoadData();
    //    FilteredItems.Refresh();
    //}


    [RelayCommand]
    private void Search()
    {
        FilteredItems.Refresh();
    }
  

}

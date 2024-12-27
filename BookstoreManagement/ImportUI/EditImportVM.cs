using BookstoreManagement.Core.Shortcut;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using BookstoreManagement.UI.ItemUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookstoreManagement.ImportUI
{
    public partial class EditImportVM : EditItemVM<Import>
    {

        private readonly ApplicationDbContext db;
        private readonly INavigatorService<AllImportVM> importNavigator;

        [ObservableProperty]
        private Import _item;


        /// Close and Open ImportItem base on visible
        [ObservableProperty]
        private bool _isImportItemVisible;

  
        // Declare in ImportItemTab
        [ObservableProperty]
        private int _itemId;

        [ObservableProperty]
        private int _quantity;

        [ObservableProperty]
        private ImportItem _selectImportItem;

        [ObservableProperty]
        private bool isIconSaveEdit;

        // This use for update state of TextBox ItemId not allow Edit when click button Edit
        [ObservableProperty]
        private bool _notAllowEdit;


        public EditImportVM(ApplicationDbContext db,
            INavigatorService<AllImportVM> importNavigator)
        {
            IsImportItemVisible = false;
            this.db = db;
            this.importNavigator = importNavigator;
        }
        // Go Back command
        [RelayCommand]
        private void GoBack()
        {
            importNavigator.Navigate();
        }

        // Load Import
        protected override void LoadItem()
        {
            Item = default;
            var id = ViewModelContext.Id;
            Item = db.Imports.Include(import => import.ImportItems).Where(i => i.Id == id).First();
        }

        protected override void SubmitItemHandler()
        {
            db.Imports.Update(Item);
            db.SaveChanges();
        }
        protected override void OnSubmittingSuccess()
        {
            base.OnSubmittingSuccess();
            MessageBox.Show("Submit Success");
        }

        // Button SelectItem in CreateImport
        [RelayCommand]
        private void SelectItem()
        {
            IsImportItemVisible = true;
        }

        // =============================================== SECTION FOR IMPORT ITEM ============================================
        // Reset state
        public override void ResetState()
        {
            base.ResetState();
            IsImportItemVisible = false; // close sidebar when navigate other tab
        }
        // Button Close Tab ImportItem
        [RelayCommand]
        private void ArrowImportItem()
        {
            ResetValue(); // Reset value in Import Item
            IsImportItemVisible = false;
        }

        // Reset ItemID and Quantity when close tab Import Item
        private void ResetValue()
        {
            ItemId = 0;
            Quantity = 0;
        }

        //  Add Item Command
        [RelayCommand]
        private void AddImportItem()
        {
            if (Quantity <= 0)
            {
                MessageBox.Show("Quantity must larger than 0", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

           
            var importItem = new ImportItem
            {
                ImportId = Item.Id,
                ItemId = ItemId,
                Quantity = Quantity
            };
            try
            {
                var existingItem = Item.ImportItems.FirstOrDefault(item => item.ItemId == ItemId);
                if (existingItem != null)
                {
                    existingItem.Quantity += Quantity;
                }
                else
                {
                    Item.ImportItems.Add(importItem);
                    LoadItem();
                }
                MessageBox.Show("Added item successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could'n add item : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ResetValue();
        }

        // Decrease quantity Button
        [RelayCommand]
        private void SubQuantity()
        {
            if (Quantity > 0)
            {
                Quantity--;
            }
        }

        // Increase quantity Button
        [RelayCommand]
        private void AddQuantity()
        {
            Quantity++;
        }



        // Import File Command
        [RelayCommand]
        private void ImportFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx",
                Title = "Select a file"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                LoadExcel(filePath);
            }
        }

        // Method Load Item from file
        private void LoadExcel(string filePath)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RowsUsed().Skip(1); // skip header line
            Item.ImportItems.Clear();
            foreach (var row in rows)
            {
                Item.ImportItems.Add(new ImportItem
                {
                    ItemId = row.Cell(1).GetValue<int>(),
                    Quantity = row.Cell(2).GetValue<int>()
                });
            }
        }

        // Edit ImportItem
        [RelayCommand]
        private void EditImportItem()
        {
            if (SelectImportItem == null)
            {
                MessageBox.Show("Please choose Item !", "Error",

                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = MessageBox.Show("Are you sure you want to edit?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                NotAllowEdit = true;
                IsIconSaveEdit = true;
                ItemId = SelectImportItem.ItemId;
                Quantity = SelectImportItem.Quantity;
            }
        }

        // SaveEdit Command
        [RelayCommand]
        private void SaveEdit()
        {
            if (SelectImportItem == null)
            {
                MessageBox.Show("Please choose Item !", "Error",

                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = MessageBox.Show("Are you sure you want to save changes?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                NotAllowEdit = false;
                IsIconSaveEdit = false;
                SelectImportItem.Quantity = Quantity;
                MessageBox.Show("Update success", "Edit", MessageBoxButton.OK);
            }
        }

        // Delete Command
        [RelayCommand]
        private void DeleteImportItem()
        {
            var result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Item.ImportItems.Remove(SelectImportItem);
                LoadItem(); // Update immediately item 
                db.SaveChanges();
            }
        }
// ===================================== END SECTION FOR IMPORT ITEM =============================================

    }
}

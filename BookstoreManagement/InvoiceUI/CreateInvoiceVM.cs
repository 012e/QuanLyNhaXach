using BookstoreManagement.Core;
using BookstoreManagement.InvoiceUI.Dtos;
using BookstoreManagement.PricingUI.Services;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class CreateInvoiceVM : BaseViewModel
{
    private readonly ApplicationDbContext db;
    private readonly INavigatorService<AllInvoicesVM> allInvoiceNavigator;

    // Hide and display tab ivoice item
    [ObservableProperty]
    private bool _isInvoiceItemVisible;

    // List use to Display InvoiceItem have Price
    [ObservableProperty]
    private ObservableCollection<InvoiceItemDto> _invoiceItemDto;

    // List use to Save into Database
    [ObservableProperty]
    private ObservableCollection<InvoicesItem> _listInvoiceItem;


    [ObservableProperty]
    private PricingService pricingService;


    [ObservableProperty]
    private decimal _total;

    [ObservableProperty]
    private int _employeeId;

    [ObservableProperty]
    private int _customerId;

    [ObservableProperty]
    private DateTime _createAt;

    // Declare in InvoiceItemTab
    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private int _quantity;

    [ObservableProperty]
    private InvoiceItemDto _selecteInvoiceItemDto;

    [ObservableProperty]
    private bool _isIconSaveEdit;

    [ObservableProperty]
    private bool _notAllowEdit;
    public CreateInvoiceVM(ApplicationDbContext db,
        INavigatorService<AllInvoicesVM> allInvoiceNavigator,
        PricingService pricingService)
    {
        this.db = db;
        this.allInvoiceNavigator = allInvoiceNavigator;
        this.pricingService = pricingService;
    }


    // Submit command
    [RelayCommand]
    private void Submit()
    {
        CreateNewInvoice();
    }

    // Go Back Command
    [RelayCommand]
    private void GoBack()
    {
        allInvoiceNavigator.Navigate();
    }

    // Reset Values TexBox in Create
    private void ResetToDefaultValues()
    {
        _total = 0;
        _employeeId = 0;
        _customerId = 0;
        CreateAt = DateTime.Now;
        IsInvoiceItemVisible = false;
    }

    // Reset State
    public override void ResetState()
    {
        base.ResetState();
        ResetToDefaultValues();
        ListInvoiceItem = new ObservableCollection<InvoicesItem>();
        InvoiceItemDto = new ObservableCollection<InvoiceItemDto>();  
    }

    // Button SelectItem In CreateInvoice
    [RelayCommand]
    private void SelectItem()
    {
        IsInvoiceItemVisible = true;
    }


    // ====================================== SECTION FOR INVOICE ITEM ===============================================
    [RelayCommand]
    private void ArrowInvoiceItem()
    {
        ResetValue(); // Reset value in Invoice Item
        IsInvoiceItemVisible = false;
    }
    // Reset ItemID and Quantity when close tab Invoice Item
    private void ResetValue()
    {
        ItemId = 0;
        Quantity = 0;
    }

  

    //  Add Item Command
    [RelayCommand]
    private void AddInvoiceItem()
    {
        if (Quantity <= 0)
        {
            MessageBox.Show("Quantity must larger than 0", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var invoiceItem = new InvoicesItem
        {
            ItemId = ItemId,
            Quantity = Quantity
        };
        try
        {
            bool check = false;
            foreach (var item in InvoiceItemDto)
            {
                if (item.id == invoiceItem.ItemId)
                {
                    item.Quantity += invoiceItem.Quantity;
                    check = true;
                    break;
                }
            }
            // If no find Item want add in InvoiceItemDto
            // Add new Item
            if (!check)
            {
                // Add InvoiceItemDto
                InvoiceItemDto.Add(new InvoiceItemDto
                {
                    id = invoiceItem.ItemId,
                    Name = db.Items.FirstOrDefault(i => i.Id == invoiceItem.ItemId)?.Name ?? "Unknow",
                    Quantity = invoiceItem.Quantity,
                    Price = pricingService.GetPrice(ItemId).FinalPrice
                });

                // Add InvoiceItem use save into Database
                ListInvoiceItem.Add(invoiceItem);
            }
            MessageBox.Show("Added item successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could'n add item : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        ResetValue();
    }

    // Edit InvoiceItem
    [RelayCommand]
    private void EditInvoiceItem()
    {
        if (SelecteInvoiceItemDto == null)
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

            // Bingding data from SelectInvoiceItem
            ItemId = SelecteInvoiceItemDto.id;
            Quantity = SelecteInvoiceItemDto.Quantity;

        }
    }

    private void CreateNewInvoice()
    {

        var newInvocie = new Invoice
        {
            EmployeeId = EmployeeId,
            CreatedAt = CreateAt,
            CustomerId = CustomerId,
            InvoicesItems = ListInvoiceItem
        };
        try
        {
            db.Add(newInvocie);
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could'n add Invoice : {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        MessageBox.Show("Added Invoice successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void SaveEdit()
    {
        if (SelecteInvoiceItemDto == null)
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
            SelecteInvoiceItemDto.Quantity += Quantity;
        }
    }

    // Delete Command
    [RelayCommand]
    private void DeleteInvoiceItem()
    {
        var result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            InvoiceItemDto.Remove(SelecteInvoiceItemDto);
            MessageBox.Show("Item removed from the list.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
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
        InvoiceItemDto.Clear();
        foreach (var row in rows)
        {
            InvoiceItemDto.Add(new InvoiceItemDto
            {
                id = row.Cell(1).GetValue<int>(),
                Quantity = row.Cell(2).GetValue<int>(),
                Price = row.Cell(3).GetValue<decimal>()
            });
        }
    }
}

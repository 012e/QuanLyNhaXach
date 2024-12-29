using BookstoreManagement.Core;
using BookstoreManagement.InvoiceUI.Dtos;
using BookstoreManagement.PricingUI.Services;
using BookstoreManagement.Shared.CustomMessages;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using BookstoreManagement.Shared.Services;
using BookstoreManagement.UI.ItemUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using ToastNotifications.Core;
using ToastNotifications.Messages.Error;

namespace BookstoreManagement.UI.InvoicesUI;

public partial class CreateInvoiceVM : BaseViewModel
{
    private readonly ApplicationDbContext db;
    private readonly INavigatorService<AllInvoicesVM> allInvoiceNavigator;

    [ObservableProperty]
    private String _searchText = "";

    [ObservableProperty]
    private ObservableCollection<Customer> _customerList;

    [ObservableProperty]
    private bool _isSet = false;
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
    private DateTime _createAt = DateTime.Now;

    // Declare in InvoiceItemTab
    [ObservableProperty]
    private int _itemId;

    

    [ObservableProperty]
    private int _quantity;

    [ObservableProperty]
    private Customer _selectedCutomer;

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
        _employeeId = 0;
        IsInvoiceItemVisible = false;
    }

    // Reset State
    public override void ResetState()
    {
        base.ResetState();
        ResetToDefaultValues();
        ListInvoiceItem = new ObservableCollection<InvoicesItem>();
        InvoiceItemDto = new ObservableCollection<InvoiceItemDto>();
        var customers = db.Customers.OrderBy(i => i.Id).ToList();
        CustomerList = new ObservableCollection<Customer>(customers);
        this.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(SearchText))
            {
                UpdateFilter();
            }
        };
        PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(SelectedCutomer))
            {
                
                IsSet = SelectedCutomer != null ? false : true;
            }
        };
    }
    private void UpdateFilter()
    {
        var customers = db.Customers.OrderBy(i => i.Id).ToList();
        var query = string.IsNullOrWhiteSpace(SearchText)
            ? new ObservableCollection<Customer>(customers)
        : new ObservableCollection<Customer>(
                customers.Where(i => i.FirstName.ToLower().Contains(SearchText.ToLower()) ||
                i.LastName.ToLower().Contains(SearchText.ToLower())||
                i.PhoneNumber.ToString().ToLower().Contains(SearchText.ToLower())));

        CustomerList = query;
    }
    [RelayCommand]
    private void OpenSetCustomer()
    {
        IsSet = true;
    }
    [RelayCommand]
    private void CloseSetCustomer()
    {
        IsSet = false;
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
            GetNotification.NotifierInstance.ErrorMessage("Error", "Quantity must larger than 0", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
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
                decimal price = pricingService.GetPrice(ItemId).FinalPrice;
                InvoiceItemDto.Add(new InvoiceItemDto
                {
                    id = invoiceItem.ItemId,
                    Name = db.Items.FirstOrDefault(i => i.Id == invoiceItem.ItemId)?.Name ?? "Unknow",
                    Quantity = invoiceItem.Quantity,
                    Price = price,
                    TotalPrice = invoiceItem.Quantity * price
                });

                // Add InvoiceItem use save into Database
                ListInvoiceItem.Add(invoiceItem);

                // Resum total Invoice
                GetTotalInvoice();
            }
            SuccessNotification();
        }
        catch 
        {
            ErrorDBNotification();
        }
        ResetValue();
    }

    // Edit InvoiceItem
    [RelayCommand]
    private void EditInvoiceItem()
    {
        if (SelecteInvoiceItemDto == null)
        {
            GetNotification.NotifierInstance.ErrorMessage("Error", "Please choose Item", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
            return;
        }
        WarningNotification();
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

    private decimal GetTotalInvoice()
    {
        // Reset value
        TotalInvoice = 0;
        foreach(var item in InvoiceItemDto)
        {
            TotalInvoice += item.TotalPrice;
        }
        return TotalInvoice;
    }

    [ObservableProperty]
    private decimal _TotalInvoice;
    private void CreateNewInvoice()
    {
        decimal total = 0;
        var newInvocie = new Invoice
        {
            EmployeeId = EmployeeId,
            CreatedAt = CreateAt,
            CustomerId = SelectedCutomer.Id,
            Total = GetTotalInvoice(),
            InvoicesItems = ListInvoiceItem
        };
        try
        {
            if(newInvocie.InvoicesItems.Count == 0)
            {
                GetNotification.NotifierInstance.ErrorMessage("Error","Not exist item, please add item", NotificationType.Error, new MessageOptions
                {
                    FreezeOnMouseEnter = false,
                    ShowCloseButton = true
                });
                return;
            }
            db.Add(newInvocie);
            db.SaveChanges();
        }
        catch
        {
            ErrorDBNotification();
            return;
        }
        SuccessNotification();
    }

    [RelayCommand]
    private void SaveEdit()
    {
        if (SelecteInvoiceItemDto == null)
        {
            GetNotification.NotifierInstance.ErrorMessage("Error", "Please choose Item !", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
            return;
        }
        WarningNotification();
        var result = MessageBox.Show("Are you sure you want to save changes?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            SuccessNotification();
            NotAllowEdit = false;
            IsIconSaveEdit = false;
            SelecteInvoiceItemDto.Quantity += Quantity;
        }
    }

    // Delete Command
    [RelayCommand]
    private void DeleteInvoiceItem()
    {
        WarningNotification();
        var result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            InvoiceItemDto.Remove(SelecteInvoiceItemDto);

            // Resum total
            GetTotalInvoice();
            GetNotification.NotifierInstance.SuccessMessage("Success", "Deleted item successfully", NotificationType.Error, new MessageOptions
            {
                FreezeOnMouseEnter = false,
                ShowCloseButton = true
            });
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
    private void WarningNotification()
    {
        GetNotification.NotifierInstance.WarningMessage("Warning", "This action cannot be undone", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
    private void ErrorDBNotification()
    {
        GetNotification.NotifierInstance.ErrorMessage("Error", "Couldn't add item: Database Error", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
    private void SuccessNotification()
    {
        GetNotification.NotifierInstance.SuccessMessage("Success", "Added item successfully", NotificationType.Error, new MessageOptions
        {
            FreezeOnMouseEnter = false,
            ShowCloseButton = true
        });
    }
}

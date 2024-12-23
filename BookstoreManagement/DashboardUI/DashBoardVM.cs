using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Dtos;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.ObjectModel;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BookstoreManagement.UI.DashboardUI
{
    public partial class DashBoardVM : BaseViewModel
    {
        private readonly ApplicationDbContext db;
        private readonly CurrentUserService currentUserService;
        [ObservableProperty]
        private string _userName;

        [ObservableProperty]
        private SeriesCollection _lineSeriesCollection;


        [ObservableProperty]
        private Func<double, string> _yFormatter;

        [ObservableProperty]
        private decimal _totalRevenue;

        [ObservableProperty]
        private decimal _totalExpense;

        [ObservableProperty]
        private double _percentProfit; // Profit per Revenue

        [ObservableProperty]
        private string _fullNameCustomer;

        [ObservableProperty]
        private List<Invoice> _recentInvoice;

        [ObservableProperty]
        private List<InvoicesItem> _bestSeller;

        public DashBoardVM(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            this.db = db;
            this.currentUserService = currentUserService;
            StartDateRevenue = DateTime.Now.AddDays(-7);
            EndDateRevenue = DateTime.Now;
        }
        public override void ResetState()
        {
            base.ResetState();
            UserName = $"Good morning, {currentUserService.CurrentUser.FirstName}";
            LoadDashBoardData();
            RevenueChart(StartDateRevenue, EndDateRevenue);
        }
        private LinearGradientBrush GradientFillChart()
        {
            var gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 1);
            gradientBrush.EndPoint = new Point(0, 0);
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 0));
            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#174CFA"), 1));
            return gradientBrush;
        }


        [ObservableProperty]
        private ObservableCollection<string> _labels;

        [ObservableProperty]
        private DateTime _startDateRevenue;

        [ObservableProperty]
        private DateTime _endDateRevenue;
        // Revenue Chart
        private void RevenueChart(DateTime startDate, DateTime endDate)
        {
            var dailyRevenue = db.Invoices.Where(i => i.CreatedAt.Date >= startDate.Date && i.CreatedAt.Date <= endDate.Date)
                .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month, i.CreatedAt.Day })
                .Select(g => new
                {
                    Day = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    Total = g.Sum(i => i.Total)
                })
                .OrderBy(g => g.Day)
                .ToList();

            LineSeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "",
                    Values = new ChartValues<double>(dailyRevenue.Select(g => (double)g.Total)),
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                    Fill = GradientFillChart()
                }
            };
            Labels = new ObservableCollection<string>(dailyRevenue.Select(g => g.Day.ToString("dd/MM/yyyy")));
            YFormatter = value => "$" + (value).ToString();
        }


        // Total Revenue
        private decimal GetToTalRevenue()
        {
            return db.Invoices.Sum(i => i.Total);
        }

        // Total Expense
        private decimal GetToTalExpense()
        {
            return db.Imports.Sum(ip => ip.TotalCost);
        }

        // Percent Profit
        private double GetPercentProfit(decimal revenue, decimal expense)
        {
            decimal profit = revenue - expense;

            // convert to double and percent
            double percentProfit = (double)profit * 100 / (double)revenue;
            return Math.Round(percentProfit, 2);
        }


        // Load Data DashBoard
        private void LoadDashBoardData()
        {
            //TotalRevenue = GetToTalRevenue();
            //TotalExpense = GetToTalExpense();
            TotalRevenue = GetToTalRevenue();
            TotalExpense = GetToTalExpense();
            PercentProfit = GetPercentProfit(TotalRevenue, TotalExpense);
            RecentInvoice = GetRecentInvoice(10);
            BestSeller = GetItemBestSeller(10);
        }

        // Recent Sale
        private List<Invoice> GetRecentInvoice(int count)
        {
            var recentInvoices = db.Invoices
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new Invoice
                {
                    Id = i.Id,
                    Total = i.Total,
                    Customer = i.Customer,
                    CreatedAt = i.CreatedAt,
                })
                .Take(count)
                .ToList();
            return recentInvoices;
        }

        // List Item Bestseller
        private List<InvoicesItem> GetItemBestSeller(int count)
        {
            // Truy van tong so luong item co trong invoice, sap xep giam dan
            var bestSeller = db.InvoicesItems
                .GroupBy(ii => ii.ItemId)
                .Select(g => new
                {
                    ItemId = g.Key,
                    TotalQuantity = g.Sum(ii => ii.Quantity),
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(count)
                .ToList();

            // Danh sach cac san pham ban chay nhat
            var itemIds = bestSeller.Select(x => x.ItemId).ToList();

            // Danh sach nay dung de lay thong tin tu itemIds tuc la cac san pham ban chay
            var items = db.Items.Where(i => itemIds.Contains(i.Id)).ToList();
            var result = bestSeller.Select(bs => new InvoicesItem
            {
                ItemId = bs.ItemId,
                Quantity = bs.TotalQuantity,
                Item = items.First(i => i.Id == bs.ItemId) // tim san pham dau tien ma co Id bestSeller = Id item
            }).ToList();

            return result;
        }

        [RelayCommand]
        private void ChangeDateRevenue()
        {
            if (StartDateRevenue > EndDateRevenue)
            {
                MessageBox.Show("Start date cannot over End date !", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ResetState();
        }
    }
}

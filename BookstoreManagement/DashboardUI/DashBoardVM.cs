using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Dtos;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
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
        private string[] _labels;

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
            LoadDashBoardData();
            RevenueChart();
        }
        public override void ResetState()
        {
            base.ResetState();
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
        private void RevenueChart()
        {
            var weeklyRevenue = db.Invoices.GroupBy(i => i.CreatedAt.DayOfWeek)
                .Select(g => new { Day = g.Key, Total = g.Sum(i => i.Total) })
                .OrderBy(g => g.Day)
                .ToList();

            var lineSeriesValue = weeklyRevenue.Select(r => (double)r.Total / 1000).ToList();
            LineSeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "",
                    Values = new ChartValues<double>{ 3133,2312,5663,9976,1383,7878,5482},
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                    Fill = GradientFillChart()
                }
            };
            Labels = new[] { "Mon", "Tue", "Wed", "Thus", "Fri", "Sat", "Sun" };
            YFormatter = value => (value / 1000).ToString("0.0" + "k");
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
            TotalRevenue = 1000;
            TotalExpense = 500;
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

    }
}

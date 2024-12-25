using BookstoreManagement.Core;
using BookstoreManagement.LoginUI.Services;
using BookstoreManagement.Shared.DbContexts;
using BookstoreManagement.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace BookstoreManagement.UI.DashboardUI
{
    public partial class DashBoardVM(ApplicationDbContext db, CurrentUserService currentUserService) : BaseViewModel
    {
        private readonly ApplicationDbContext db = db;
        private readonly CurrentUserService currentUserService = currentUserService;

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
        private IEnumerable<ISeries> _percentProfit = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(
                30,
                series =>
                {
                    series.MaxRadialColumnWidth = 50;
                    series.DataLabelsSize = 33;
                })
            );

        [ObservableProperty]
        private string _fullNameCustomer;

        [ObservableProperty]
        private List<Invoice> _recentInvoice;

        [ObservableProperty]
        private List<InvoicesItem> _bestSeller;

        [ObservableProperty]
        private bool _loading = false;

        [ObservableProperty]
        private ObservableCollection<string> _labels;

        [ObservableProperty]
        private DateTime _startDateRevenue = DateTime.Now.AddDays(-7);

        [ObservableProperty]
        private DateTime _endDateRevenue = DateTime.Now;

        partial void OnStartDateRevenueChanged(DateTime oldValue, DateTime newValue)
        {
            if (StartDateRevenue > EndDateRevenue)
            {
                MessageBox.Show("Start date must be before End date!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StartDateRevenue = oldValue;
                return;
            }
            LoadDashBoardData();
        }

        partial void OnEndDateRevenueChanged(DateTime oldValue, DateTime newValue)
        {
            if (StartDateRevenue > EndDateRevenue)
            {
                MessageBox.Show("Start date cannot over End date !", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                EndDateRevenue = oldValue;
                return;
            }
            LoadDashBoardData();
        }

        private void SetPercentProfit(double percent)
        {
            PercentProfit = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(
                percent,
                series =>
                {
                    series.MaxRadialColumnWidth = 50;
                    series.DataLabelsSize = 25;
                    series.Fill = new SolidColorPaint(new SKColor(65, 145, 221));
                    series.DataLabelsPaint = new SolidColorPaint(SKColors.Black);
                })
            );
        }
        private void ResetToDefaults()
        {
            StartDateRevenue = DateTime.Now.AddDays(-7);
            EndDateRevenue = DateTime.Now;
            TotalExpense = 0;
            TotalRevenue = 0;
            SetPercentProfit(0);
            LineSeriesCollection = [];
        }

        public override void ResetState()
        {
            base.ResetState();
            ResetToDefaults();
            UserName = @$"Good morning, {currentUserService.CurrentUser?.FirstName ?? ""}";
            LoadDashBoardDataCommand.Execute(null);
        }

        private LinearGradientBrush GradientFillChart()
        {
            var gradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 1),
                EndPoint = new Point(0, 0)
            };
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 0));
            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#174CFA"), 1));
            return gradientBrush;
        }

        private async Task LoadRevenueChart(DateTime startDate, DateTime endDate)
        {
            var dailyRevenue = await db.Invoices
                .Where(i => i.CreatedAt.Date >= startDate.Date && i.CreatedAt.Date <= endDate.Date)
                .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month, i.CreatedAt.Day })
                .Select(g => new
                {
                    Day = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    Total = g.Sum(i => i.Total)
                })
                .OrderBy(g => g.Day)
                .ToListAsync();

            LineSeriesCollection =
            [
                new LineSeries
                {
                    Title = "",
                    Values = new ChartValues<double>(dailyRevenue.Select(g => (double)g.Total)),
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                    Fill = GradientFillChart()
                }
            ];
            Labels = new ObservableCollection<string>(dailyRevenue.Select(g => g.Day.ToString("dd/MM/yyyy")));
            YFormatter = value => "$" + (value).ToString();
        }

        // Total Revenue
        private async Task<decimal> GetTotalRevenue()
        {
            return await db.Invoices.SumAsync(i => i.Total);
        }

        // Total Expense
        private async Task<decimal> GetTotalExpense()
        {
            return await db.Imports.SumAsync(ip => ip.TotalCost);
        }

        // Percent Profit
        private double GetPercentProfit(decimal revenue, decimal expense)
        {
            decimal profit = revenue - expense;

            // convert to double and percent
            double percentProfit = (double)profit * 100 / (double)revenue;
            return Math.Round(percentProfit, 2);
        }


        [RelayCommand]
        private async Task LoadDashBoardData()
        {
            if (Loading)
            {
                return;
            }
            Loading = true;
            TotalRevenue = await GetTotalRevenue();
            TotalExpense = await GetTotalExpense();
            SetPercentProfit(GetPercentProfit(TotalRevenue, TotalExpense));
            RecentInvoice = await GetRecentInvoice(10);
            BestSeller = await GetItemBestSeller(10);
            await LoadRevenueChart(StartDateRevenue, EndDateRevenue);
            Loading = false;
        }

        // Recent Sale
        private async Task<List<Invoice>> GetRecentInvoice(int count)
        {
            var recentInvoices = await db.Invoices
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new Invoice
                {
                    Id = i.Id,
                    Total = i.Total,
                    Customer = i.Customer,
                    CreatedAt = i.CreatedAt,
                })
                .Take(count)
                .ToListAsync();
            return recentInvoices;
        }

        // List Item Bestseller
        private async Task<List<InvoicesItem>> GetItemBestSeller(int count)
        {
            // Truy van tong so luong item co trong invoice, sap xep giam dan
            var bestSeller = await db.InvoicesItems
                .GroupBy(ii => ii.ItemId)
                .Select(g => new
                {
                    ItemId = g.Key,
                    TotalQuantity = g.Sum(ii => ii.Quantity),
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(count)
                .ToListAsync();

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

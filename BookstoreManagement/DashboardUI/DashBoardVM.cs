using BookstoreManagement.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Bibliography;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace BookstoreManagement.UI.DashboardUI
{
    public partial class DashBoardVM : BaseViewModel
    {
        [ObservableProperty]
        private SeriesCollection _lineSeriesCollection;

        [ObservableProperty]
        private SeriesCollection _doughnutSeriesCollection;

        [ObservableProperty]
        private string[] _labels;

        [ObservableProperty]
        private Func<double, string> _yFormatter;


        public DashBoardVM()
        {
            RevenueChart();
            ProfitChart();
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
            LineSeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "",
                    Values = new ChartValues<double> {4,5,6,2,1,10,5},
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                    Fill = GradientFillChart()
                }
            };
            Labels = new[] { "Mon", "Tue", "Wed", "Thus", "Fri", "Sat", "Sun" };
            YFormatter = value => value.ToString("C");
        }

        private void ProfitChart()
        {
            DoughnutSeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Chrome",
                    Values = new ChartValues<ObservableValue> {new ObservableValue(8)},
                    DataLabels = true
                },
                 new PieSeries
                {
                    Title = "asc",
                    Values = new ChartValues<ObservableValue> {new ObservableValue(6)},
                    DataLabels = true

                },
                  new PieSeries
                {
                    Title = "Cqwe",
                    Values = new ChartValues<ObservableValue> {new ObservableValue(10)},
                    DataLabels = true
                }
            };
        }
      
    }
}

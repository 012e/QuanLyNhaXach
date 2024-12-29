using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications.Core;

namespace BookstoreManagement.Shared.CustomMessages
{
    /// <summary>
    /// Interaction logic for CustomDisplayPart.xaml
    /// </summary>
    public partial class CustomDisplayPart : NotificationDisplayPart
    {
        
        private void StartTimeBarAnimation()
        {
            var duration = new Duration(TimeSpan.FromSeconds(3));
            var widthAnimation = new DoubleAnimation
            {
                From = TimeBar.ActualWidth,
                To = 0,
                Duration = duration
            };

            TimeBar.BeginAnimation(WidthProperty, widthAnimation);
        }
        public CustomDisplayPart(CustomNotification customNotification)
        {
            InitializeComponent();
            Bind(customNotification);

            TimeBar.Loaded += TimeBar_Loaded;
        }

        private void TimeBar_Loaded(object sender, RoutedEventArgs e)
        {
            StartTimeBarAnimation();
        }
    }
}

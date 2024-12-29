using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace BookstoreManagement.Shared.CustomMessages
{
    public class GetNotification
    {
        private static Notifier _notifier;

        // Hàm khởi tạo singleton
        public static Notifier NotifierInstance
        {
            get
            {
                if (_notifier == null)
                {
                    _notifier = new Notifier(cfg =>
                    {
                        cfg.PositionProvider = new WindowPositionProvider(
                            parentWindow: Application.Current.MainWindow,
                            corner: Corner.BottomRight,
                            offsetX: 10,
                            offsetY: 10);

                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                            notificationLifetime: TimeSpan.FromSeconds(3),
                            maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                        cfg.DisplayOptions.Width = 350;
                        cfg.Dispatcher = Application.Current.Dispatcher;
                    });
                }

                return _notifier;
            }
        }
        public static void DisposeNotifier()
        {
            _notifier?.Dispose();
            _notifier = null;
        }
    }
}


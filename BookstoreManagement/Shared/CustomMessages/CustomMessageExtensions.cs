using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Core;

namespace BookstoreManagement.Shared.CustomMessages
{
    public static class CustomMessageExtensions
    {
        public static void SuccessMessage(this Notifier notifier,
            string title,
            string message,
            NotificationType type,
            MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new CustomNotification(title, message,type,
                messageOptions, "#D4F9E7", "#23D578", "#23D578", "pack://application:,,,/Shared/Images/success_Icon.png"));
        }

        public static void ErrorMessage(this Notifier notifier,
           string title,
           string message,
           NotificationType type,
           MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new CustomNotification(title, message, type,
                messageOptions, "#FFE5E5", "#D8535D", "#D8535D", "pack://application:,,,/Shared/Images/Error_Icon.png"));
        }

        public static void InfoMessage(this Notifier notifier,
           string title,
           string message,
           NotificationType type,
           MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new CustomNotification(title, message, type,
                messageOptions, "#EAF0FA", "#2383F4", "#2383F4", "pack://application:,,,/Shared/Images/info.png"));
        }

        public static void WarningMessage(this Notifier notifier,
           string title,
           string message,
           NotificationType type,
           MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new CustomNotification(title, message, type,
                messageOptions, "#FDF3DC", "#EF9E11", "#EF9E11", "pack://application:,,,/Shared/Images/warning.png"));
        }
    }
}

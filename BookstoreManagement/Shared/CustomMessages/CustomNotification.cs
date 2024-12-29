using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications.Core;

namespace BookstoreManagement.Shared.CustomMessages
{
    public enum NotificationType
    {
        Success,
        Error,
        Info
    }
    public class CustomNotification : NotificationBase, INotifyPropertyChanged
    {
        private CustomDisplayPart _displayPart;

        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new CustomDisplayPart(this));

        public NotificationType Type { get; }
        
        public CustomNotification(string title, string message, NotificationType type, MessageOptions messageOptions,
            string backColor, string forceColor,string timeBarColor, string path) : base(message, messageOptions)
        {
            Title = title;
            Message = message;
            Type = type;
            CodeColorBG = backColor;
            CodeColorT = forceColor;
            CodeColorTimeBar = timeBarColor;
            IconPath = path;

        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private string _codeColorBG;
        public string CodeColorBG
        {
            get
            {
                return _codeColorBG;
            }
            set
            {
                _codeColorBG = value;
                OnPropertyChanged();
            }
        }

        private string _codeColorT;
        public string CodeColorT
        {
            get
            {
                return _codeColorT;
            }
            set
            {
                _codeColorT = value;
                OnPropertyChanged();
            }
        }

        private string _codeColorTimeBar;
        public string CodeColorTimeBar
        {
            get
            {
                return _codeColorTimeBar;
            }
            set
            {
                _codeColorTimeBar = value;
                OnPropertyChanged();
            }
        }

        private string _iconPath;
        public string IconPath
        {
            get
            {
                return _iconPath;
            }
            set
            {
                _iconPath = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

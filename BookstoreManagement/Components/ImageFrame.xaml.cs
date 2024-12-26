using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BookstoreManagement.Components
{
    /// <summary>
    /// Interaction logic for ImageFrame.xaml
    /// </summary>
    public partial class ImageFrame : UserControl
    {
        public ImageFrame()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageFrame), new PropertyMetadata(null));

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ImageFrame), new PropertyMetadata(new CornerRadius(0)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(ImageFrame), new PropertyMetadata(null, OnCommandChanged));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageFrame control && e.NewValue is IAsyncCommand asyncCommand)
            {
                control.Command = asyncCommand;
            }
        }

        public static readonly DependencyProperty ControlWidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ImageFrame), new PropertyMetadata(100.0));

        public static readonly DependencyProperty ControlHeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ImageFrame), new PropertyMetadata(100.0));

        public new double Width
        {
            get => (double)GetValue(ControlWidthProperty);
            set => SetValue(ControlWidthProperty, value);
        }

        public new double Height
        {
            get => (double)GetValue(ControlHeightProperty);
            set => SetValue(ControlHeightProperty, value);
        }
    }

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
        bool CanExecute(object parameter);
    }

    public class AsyncCommand : IAsyncCommand
    {
        private readonly Func<object, Task> _execute;
        private readonly Predicate<object> _canExecute;

        public AsyncCommand(Func<object, Task> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public Task ExecuteAsync(object parameter)
        {
            return _execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

    }
}

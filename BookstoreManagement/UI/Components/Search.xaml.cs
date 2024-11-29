using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookstoreManagement.UI.Components;

/// <summary>
/// Interaction logic for Search.xaml
/// </summary>
public partial class Search : UserControl
{
    public String Text
    {
        get { return (String)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public ICommand Command
    {
        get { return (ICommand)GetValue(MyCommandProperty); }
        set { SetValue(MyCommandProperty, value); }
    }

    public static readonly DependencyProperty MyCommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(Search), new FrameworkPropertyMetadata(default(ICommand),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(String), typeof(Search), new FrameworkPropertyMetadata(default(String),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public String PlaceHolder
    {
        get { return (String)GetValue(PlaceHolderProperty); }
        set { SetValue(PlaceHolderProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PlaceHolder.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PlaceHolderProperty =
        DependencyProperty.Register("PlaceHolder", typeof(String), typeof(Search), new FrameworkPropertyMetadata(default(String)));

    public Search()
    {
        InitializeComponent();
    }
}

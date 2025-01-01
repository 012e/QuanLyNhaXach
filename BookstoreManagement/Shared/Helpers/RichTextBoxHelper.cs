using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace BookstoreManagement.Shared.Helpers
{
    public static class RichTextBoxHelper
    {
        public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.RegisterAttached("PlaceholderText", typeof(string), typeof(RichTextBoxHelper),
            new PropertyMetadata(string.Empty, OnPlaceholderTextChanged));

        public static string GetPlaceholderText(DependencyObject obj)
        {
            return (string)obj.GetValue(PlaceholderTextProperty);
        }

        public static void SetPlaceholderText(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderTextProperty, value);
        }

        private static void OnPlaceholderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox richTextBox)
            {
                richTextBox.GotFocus += (sender, args) =>
                {
                    TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                    if (textRange.Text.Trim() == (string)e.NewValue)
                    {
                        richTextBox.Document.Blocks.Clear();
                    }
                };

                richTextBox.LostFocus += (sender, args) =>
                {
                    TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                    if (string.IsNullOrWhiteSpace(textRange.Text))
                    {
                        richTextBox.Document.Blocks.Clear();
                        richTextBox.Document.Blocks.Add(new Paragraph(new Run((string)e.NewValue))
                        {
                            Foreground = Brushes.Gray
                        });
                    }
                };

                richTextBox.LostFocus += (sender, args) =>
                {
                    TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                    if (string.IsNullOrWhiteSpace(textRange.Text))
                    {
                        richTextBox.Document.Blocks.Clear();
                        richTextBox.Document.Blocks.Add(new Paragraph(new Run((string)e.NewValue))
                        {
                            Foreground = Brushes.Gray
                        });
                    }
                };
            }
        }
    }
}

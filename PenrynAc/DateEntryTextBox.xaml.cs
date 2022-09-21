using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PenrynAc;

public partial class DateEntryTextBox
{
    private DateTime _value;
    private bool _hasValue;
    public event EventHandler? ValueChanged;

    public DateEntryTextBox()
    {
        InitializeComponent();
        Clear();
    }

    private void TextboxDate_TextChanged(object sender, TextChangedEventArgs e)
    {
        string q = TextboxDate.Text.Trim();
        if (String.IsNullOrWhiteSpace(q))
        {
            TextboxDate.Opacity = 0.5;
            _hasValue = false;
            TextboxDate.ToolTip = "Null date";
            TextblockResult.Text = "Null date";
        }
        else
        {
            TextboxDate.Opacity = 1;
            if (DateTime.TryParse(q, out _value))
            {
                TextboxDate.Foreground = Brushes.Black;
                _hasValue = true;

                TextboxDate.ToolTip = _value.ToString("dd MMM yyyy", CultureInfo.CurrentCulture);
                TextblockResult.Text = _value.ToString("dd MMM yyyy", CultureInfo.CurrentCulture);
            }
            else
            {
                TextboxDate.Foreground = Brushes.Red;
                _hasValue = false;
                TextboxDate.ToolTip = "Null date";
                TextblockResult.Text = "Null date";
            }
        }

        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public DateTime? DateValue
    {
        get
        {
            if (_hasValue)
            {
                return _value;
            }

            return null;
        }
        set
        {
            _value = value ?? DateTime.MinValue;
            _hasValue = value.HasValue;
            if (_value.Ticks < 1)
            {
                TextboxDate.Clear();
            }
            else
            {
                TextboxDate.Text = _value.ToShortDateString();
            }

        }
    }

    private void Clear()
    {
        _value = new DateTime(1954, 1, 3);
        _hasValue = false;
        TextboxDate.Clear();
    }

    private void TextboxDate_GotFocus(object sender, RoutedEventArgs e)
    {
        GridBase.Background = Brushes.White;
    }

    private void TextboxDate_LostFocus(object sender, RoutedEventArgs e)
    {
        GridBase.Background = Brushes.WhiteSmoke;
    }

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        TextblockResult.Text = string.Empty;
    }
}
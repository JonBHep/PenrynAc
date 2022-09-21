using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PenrynAc;

public partial class DateChooserWindow
{
   private readonly string _dateFormat = "dd MMMM yyyy, dddd";
        private int _selMonth ;
        private int _selDay;
        private DateTime _selDate = DateTime.Today;
        private readonly int[] _monthMaxDays = new int[13];

        public DateChooserWindow()
        {
            InitializeComponent();
        }

        public DateTime SelectedDate => _selDate;

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            Restart();
        }

        private void Restart()
        {
            TextblockToday.Text = "Today is " + DateTime.Today.ToString(_dateFormat, CultureInfo.CurrentCulture);
            _selMonth = _selDay = 0;

            BuildDateSelectionList();

            ListboxPicker.Items.Clear();
            for (int d = 1; d < 32; d++)
            {
                ListBoxItem it = new ListBoxItem()
                {
                    Tag = d
                };
                TextBlock tb = new TextBlock()
                {
                    Text = d.ToString(CultureInfo.CurrentCulture),
                    Width = 60,
                    Foreground = new SolidColorBrush(Colors.Blue)
                };
                it.Content = tb;
                ListboxPicker.Items.Add(it);
            }
            ButtonReset.IsEnabled = false;
            ButtonSelect.IsEnabled = false;

            TextblockSelectedDate.Text = string.Empty;
        }

        private void BuildDateSelectionList()
        {
            ListboxDates.Items.Clear();
            if (_selDay < 1)
            {
                // Show the last 60 days
                for (int d = 0; d < 60; d++)
                {
                    DateTime dy = DateTime.Today.AddDays(0 - d);
                    ListBoxItem it = new ListBoxItem()
                    {
                        Tag = dy
                    };
                    TextBlock tb = new TextBlock()
                    {
                        Text = dy.ToString(_dateFormat, CultureInfo.CurrentCulture)
                    };
                    if (tb.Text.Contains("S")) { tb.Foreground = new SolidColorBrush(Colors.Blue); tb.FontWeight = FontWeights.Medium; } else { tb.Foreground = new SolidColorBrush(Colors.DarkBlue); tb.FontWeight = FontWeights.Normal; }
                    it.Content = tb;
                    ListboxDates.Items.Add(it);
                }
            }
            else if (_selMonth < 1)
            {
                // Only a day has been selected, show the eligible dates
                int xYr = DateTime.Today.Year;
                while (xYr > 2011)
                {
                    for (int xMh = 12; xMh > 0; xMh--)
                    {
                        if (IsOkDate(xYr, xMh, _selDay))
                        {
                            string datestring = $"{_selDay}/{xMh}/{xYr}";

                            DateTime xDate = DateTime.Parse(datestring, CultureInfo.CurrentCulture);
                            ListBoxItem it = new ListBoxItem()
                            {
                                Tag = xDate
                            };
                            TextBlock tb = new TextBlock()
                            {
                                Text = xDate.ToString(_dateFormat, CultureInfo.CurrentCulture)
                            };
                            if (tb.Text.Contains("S")) { tb.Foreground = new SolidColorBrush(Colors.Blue); tb.FontWeight = FontWeights.Medium; } else { tb.Foreground = new SolidColorBrush(Colors.DarkBlue); tb.FontWeight = FontWeights.Normal; }
                            it.Content = tb;
                            ListboxDates.Items.Add(it);
                        }
                    }
                    xYr--;
                }

            }
            else
            {
                // Day and month have been selected, show the eligible dates
                int xYr = DateTime.Today.Year;
                while (xYr > 2011)
                {
                    if (IsOkDate(xYr, _selMonth, _selDay))
                    {
                        string datestring = $"{_selDay}/{_selMonth}/{xYr}";
                        DateTime xDate = DateTime.Parse(datestring, CultureInfo.CurrentCulture);
                        ListBoxItem it = new ListBoxItem()
                        {
                            Tag = xDate
                        };
                        TextBlock tb = new TextBlock()
                        {
                            Text = xDate.ToString(_dateFormat, CultureInfo.CurrentCulture)
                        };
                        if (tb.Text.Contains("S")) { tb.Foreground = new SolidColorBrush(Colors.Blue); tb.FontWeight = FontWeights.Medium; } else { tb.Foreground = new SolidColorBrush(Colors.DarkBlue); tb.FontWeight = FontWeights.Normal; }
                        it.Content = tb;
                        ListboxDates.Items.Add(it);
                    }
                    xYr--;
                }
            }

        }

        private void SetSelectedDate(DateTime sd)
        {
            _selDate = sd;
            TextblockSelectedDate.Text = _selDate.ToString(_dateFormat, CultureInfo.CurrentCulture);
            ButtonSelect.IsEnabled = true;
            ListboxPicker.Items.Clear();
            ListboxDates.Items.Clear();
            ButtonReset.IsEnabled = true;
        }

        private void ListboxDates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListboxDates.SelectedItem == null) { return; }
            ListBoxItem it = (ListBoxItem)ListboxDates.SelectedItem;
            DateTime dt = (DateTime)it.Tag;
            SetSelectedDate(dt);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            _monthMaxDays[1] = _monthMaxDays[3] = _monthMaxDays[5] = _monthMaxDays[7] = _monthMaxDays[8] = _monthMaxDays[10] = _monthMaxDays[12] = 31;
            _monthMaxDays[2] = 29;
            _monthMaxDays[4] = _monthMaxDays[6] = _monthMaxDays[9] = _monthMaxDays[11] = 30;
            Restart();
        }

        private void ListboxPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListboxPicker.SelectedItem == null) { return; }
            if (_selDay == 0) // select day and display months
            {
                ListBoxItem it = (ListBoxItem)ListboxPicker.SelectedItem;
                int i = (int)it.Tag;
                _selDay = i;
                TextblockSelectedDate.Text = _selDay.ToString(CultureInfo.CurrentCulture);
                ListboxPicker.Items.Clear();
                for (int m = 1; m < 13; m++)
                {
                    if (_monthMaxDays[m] >= _selDay)
                    {
                        ListBoxItem mit = new ListBoxItem()
                        {
                            Tag = m
                        };
                        TextBlock tb = new TextBlock()
                        {
                            Text = MonthName(m),
                            Foreground = new SolidColorBrush(Colors.Blue)
                        };
                        mit.Content = tb;
                        ListboxPicker.Items.Add(mit);
                    }
                }
                ButtonReset.IsEnabled = true;
                BuildDateSelectionList();
                return;
            }
            if (_selMonth == 0) // select month and display years
            {
                ListBoxItem it = (ListBoxItem)ListboxPicker.SelectedItem;
                int i = (int)it.Tag;
                _selMonth = i;
                TextblockSelectedDate.Text = $"{_selDay} {MonthName(_selMonth)}";
                ListboxPicker.Items.Clear();
                for (int y = DateTime.Today.Year; y > 2011; y--)
                {
                    if (IsOkDate(y, _selMonth, _selDay))
                    {
                        ListBoxItem dit = new ListBoxItem()
                        {
                            Tag = y
                        };
                        TextBlock tb = new TextBlock()
                        {
                            Text = y.ToString(CultureInfo.CurrentCulture),
                            Foreground = new SolidColorBrush(Colors.Blue)
                        };
                        dit.Content = tb;
                        ListboxPicker.Items.Add(dit);
                    }
                }
                ButtonReset.IsEnabled = true;
                BuildDateSelectionList();
                return;
            }
            // year selection
            ListBoxItem itm = (ListBoxItem)ListboxPicker.SelectedItem;
            int yr = (int)itm.Tag;
            DateTime sdt = new DateTime(yr, _selMonth, _selDay);
            SetSelectedDate(sdt);
            BuildDateSelectionList();
        }

        private bool IsOkDate(int y, int m, int d)
        {
            string attempt = $"{d}/{m}/{y}";
            bool q = DateTime.TryParse(attempt, out DateTime result);
            if (!q) { return false; }
            if (CheckboxFuture.IsChecked.HasValue && CheckboxFuture.IsChecked.Value)
            {
                return true;
            }
            else
            {
                if (result.Date > DateTime.Today.Date) { return false; } else { return true; }
            }
        }

        private static string MonthName(int monthNumber)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month: monthNumber);
        }
}
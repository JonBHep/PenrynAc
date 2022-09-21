using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PenrynAc;

public partial class PropertySharesWindow
{
    public PropertySharesWindow(string propertyTitle)
        {
            InitializeComponent();
            _propertyName = propertyTitle;
        }

        private readonly SharingSchedule _localSchedule = new SharingSchedule();
        private readonly string _propertyName;
        private double _firstProportion = 1;

        public string OutputScheduleSpecification => _localSchedule.Specification;

        public void SetSchedule(string spec)
        {
            _localSchedule.Specification = spec;
            DisplaySchedule();
        }

        private void DisplaySchedule()
        {
            TbkProperty.Text = _propertyName;
            StartDateDatePicker.SelectedDate = DateTime.Today;
            NumeratorTextBox.Text = "";
            DenominatorTextBox.Text = "";
            SharePhasesListBox.Items.Clear();
            foreach (Tuple<DateTime, double> phase in _localSchedule.Schedule)
            {
                TextBlock tbDate = new TextBlock() { Text = "From " + phase.Item1.ToShortDateString() };
                TextBlock tbProp = new TextBlock() { Text = " First landlord has " + phase.Item2.ToString(CultureInfo.CurrentCulture) };
                StackPanel spnl = new StackPanel() { Orientation = Orientation.Horizontal };
                spnl.Children.Add(tbDate);
                spnl.Children.Add(tbProp);
                ListBoxItem lbItem = new ListBoxItem() { Content = spnl };
                SharePhasesListBox.Items.Add(lbItem);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            string num = NumeratorTextBox.Text;
            string den = DenominatorTextBox.Text;
            bool flag = false;
            if (int.TryParse(num, out int numi))
            {
                if ((numi > 0) && (int.TryParse(den, out int deni)))
                {
                    if (deni > 0)
                    {
                        _firstProportion = numi / (double)deni;
                        ProportionTextBlock.Text = _firstProportion.ToString(CultureInfo.CurrentCulture);
                        ProportionTextBlock.Foreground = Brushes.Blue;
                        AddPhaseButton.IsEnabled = true;
                    }
                    else { flag = true; }
                }
                else { flag = true; }
            }
            else { flag = true; }
            if (flag)
            {
                ProportionTextBlock.Text = "Error";
                ProportionTextBlock.Foreground = Brushes.Red;
                AddPhaseButton.IsEnabled = false;
            }
        }

        private void AddPhaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartDateDatePicker.SelectedDate.HasValue)
            {
                DateTime dt = StartDateDatePicker.SelectedDate.Value;
                _localSchedule.Schedule.Add(new Tuple<DateTime, double>(dt, _firstProportion));
                DisplaySchedule();
            }
            else
            {
                MessageBox.Show("Select a date", Jbh.AppManager.AppName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void SharePhasesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DelPhaseButton.IsEnabled = (SharePhasesListBox.SelectedIndex >= 0);
        }

        private void DelPhaseButton_Click(object sender, RoutedEventArgs e)
        {
            _localSchedule.Schedule.RemoveAt(SharePhasesListBox.SelectedIndex);
            DisplaySchedule();
        }
}
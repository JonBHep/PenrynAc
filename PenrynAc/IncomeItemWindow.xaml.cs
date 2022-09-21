using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PenrynAc;

public partial class IncomeItemWindow
{
    public bool ParamNewItem { get; set; }
    public IncomeItem Ink { get; set; }
    public bool DunLoading { get; set; }

    public IncomeItemWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LblAmountInterpret.Text = string.Empty;
        if (ParamNewItem)
        {
            Ink = new IncomeItem(whenReceived: DateTime.Today, description: string.Empty, penceValue: 0
                , periodFrom: DateTime.Today, periodTo: DateTime.Today, furnishd: false);
        }

        ReceivedDateTextBlock.Tag = Ink.DateReceived;
        ReceivedDateTextBlock.Text = Ink.DateReceived.ToShortDateString();
        TxtAmount.Text = (Ink.AmountPence / (decimal) 100).ToString("0.00", CultureInfo.CurrentCulture);
        TxtDescription.Text = Ink.Rubric;
        FromDateTextBlock.Tag = Ink.CoversPeriodFromDate;
        FromDateTextBlock.Text = Ink.CoversPeriodFromDate.ToShortDateString();
        ToDateTextBlock.Tag = Ink.CoversPeriodToDate;
        ToDateTextBlock.Text = Ink.CoversPeriodToDate.ToShortDateString();
        FurnishedCheckBox.IsChecked = Ink.Furnished;
        ShowDayCount();
        DunLoading = true;
    }

    private void ShowDayCount()
    {
        int dys = Ink.DaysCovered;
        LblDaysCovered.Text = $"{dys} days";
        if (dys > 0)
        {
            LblDaysCovered.Foreground = Brushes.Blue;
        }
        else
        {
            LblDaysCovered.Foreground = Brushes.Red;
        }
    }

    private void TxtAmount_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Identical method in ExpenditureItem window
        string qs = TxtAmount.Text.Trim();
        if (string.IsNullOrWhiteSpace(qs))
        {
            LblAmountInterpret.Text = "Amount is missing";
            LblAmountInterpret.Foreground = Brushes.Red;
            Ink.AmountPence = 0;
        }
        else
        {
            if (decimal.TryParse(qs, out decimal sAmount))
            {
                LblAmountInterpret.Text = sAmount.ToString("C", CultureInfo.CurrentCulture);
                if (sAmount <= 0)
                {
                    LblAmountInterpret.Foreground = Brushes.Red;
                    Ink.AmountPence = Convert.ToInt32(sAmount * 100); // allowed but will be queried
                }
                else
                {
                    LblAmountInterpret.Foreground = Brushes.Blue;
                    Ink.AmountPence = Convert.ToInt32(sAmount * 100); // allowed but will be queried
                }
            }
            else
            {
                LblAmountInterpret.Text = "Amount is not a valid number";
                LblAmountInterpret.Foreground = Brushes.Red;
                Ink.AmountPence = 0;
            }
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (Ink.AmountPence <= 0)
        {
            JbhMessage("Error in amount");
            return;
        }

        if (string.IsNullOrWhiteSpace(Ink.Rubric))
        {
            JbhMessage("You must enter a description");
            return;
        }

        if (Ink.DateReceived > DateTime.Today)
        {
            JbhMessage("Date received is in the future");
            return;
        }

        if (Ink.DateReceived < new DateTime(year: 2005, month: 4, day: 6))
        {
            JbhMessage("Date received is too long ago");
            return;
        }

        if (Ink.CoversPeriodFromDate > DateTime.Today)
        {
            JbhMessage("'Covers from' date is in the future");
            return;
        }

        if (Ink.CoversPeriodFromDate < new DateTime(year: 2005, month: 4, day: 6))
        {
            JbhMessage("'Covers from' date is too long ago");
            return;
        }

        if (Ink.CoversPeriodToDate < Ink.CoversPeriodFromDate)
        {
            JbhMessage("'Covers to' date is before 'Covers from' date");
            return;
        }

        if (Ink.CoversPeriodToDate > DateTime.Today.AddYears(1))
        {
            JbhMessage("'Covers to' date is too far ahead");
            return;
        }

        if (Ink.DaysCovered < 1)
        {
            MessageBoxResult answ = JbhMessage("Zero 'covers' period", MessageBoxButton.OKCancel
                , MessageBoxImage.Question);
            if (answ == MessageBoxResult.Cancel)
            {
                return;
            }
        }

        Core.LastIncItemRubric = TxtDescription.Text;
        Core.LastIncItemAmount = Ink.AmountPence;
        DateTime datum = new DateTime(2016, 4, 6);
        if (Ink.CoversPeriodToDate > datum)
        {
            if (FurnishedCheckBox.IsChecked.HasValue && FurnishedCheckBox.IsChecked.Value)
            {
                JbhMessage(
                    "The 'Covers to' date is after 6/4/2016 so the 'Furnished' checkbox should not be ticked as wear and tear allowance was abolished from that date."
                    , MessageBoxButton.OK);
            }
        }

        DialogResult = true;
    }

    private static MessageBoxResult JbhMessage(string rubric, MessageBoxButton bouton = MessageBoxButton.OK
        , MessageBoxImage pic = MessageBoxImage.Asterisk)
    {
        return MessageBox.Show(rubric, Jbh.AppManager.AppName, bouton, pic);
    }

    private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!DunLoading) return;
        Ink.Rubric = TxtDescription.Text.Trim();
    }

    // private void FromDateChanged(object sender, EventArgs e)
    // {
    //     if (!DunLoading) return;
    //     if (!DateboxFrom.DateValue.HasValue)
    //     {
    //         return;
    //     }
    //
    //     Ink.CoversPeriodFromDate = DateboxFrom.DateValue.Value;
    //     ShowDayCount();
    // }

    // private void ToDateChanged(object sender, EventArgs e)
    // {
    //     if (!DunLoading) return;
    //     if (!DateboxTo.DateValue.HasValue)
    //     {
    //         return;
    //     }
    //
    //     Ink.CoversPeriodToDate = DateboxTo.DateValue.Value;
    //     ShowDayCount();
    // }

    // private void ReceivedDateChanged(object sender, EventArgs e)
    // {
    //     if (!DunLoading) return;
    //     if (DateboxReceived.DateValue.HasValue)
    //     {
    //         Ink.ExceptionallySetReceivedDate(DateboxReceived.DateValue.Value);
    //     }
    // }

    private void ReceivedDateButton_Click(object sender, RoutedEventArgs e)
    {
        DateChooserWindow w = new DateChooserWindow() {Owner = this};
        if (w.ShowDialog() == true)
        {
            DateTime d = w.SelectedDate;
            ReceivedDateTextBlock.Tag = d;
            ReceivedDateTextBlock.Text = d.ToShortDateString();
            Ink.ExceptionallySetReceivedDate(d);
        }
    }

    private void ChkFurnished_Checked(object sender, RoutedEventArgs e)
    {
        if (!DunLoading) return;
        Ink.Furnished = (bool) FurnishedCheckBox.IsChecked;
    }

    private void ChkFurnished_Unchecked(object sender, RoutedEventArgs e)
    {
        if (!DunLoading) return;
        Ink.Furnished = (bool) FurnishedCheckBox.IsChecked;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void BtnCoverMonth_Click(object sender, RoutedEventArgs e)
    {
        if (ReceivedDateTextBlock.Tag is DateTime got)
        {
            DateTime s = new DateTime(year: got.Year, month: got.Month, day: 1);
            DateTime f = s.AddMonths(1);
            f = f.AddDays(-1);
            FromDateTextBlock.Tag = s;
            FromDateTextBlock.Text = s.ToShortDateString();
            Ink.CoversPeriodFromDate = s;
            ToDateTextBlock.Tag = f;
            ToDateTextBlock.Text = f.ToShortDateString();
            Ink.CoversPeriodToDate = f;
        }
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        //DateboxReceived.Focus();
        if (string.IsNullOrWhiteSpace(Core.LastIncItemRubric))
        {
            ButtonPaste.IsEnabled = false;
        }
        else
        {
            ButtonPaste.IsEnabled = true;
            TextblockLastDescription.Text = $"{Core.LastIncItemRubric} £{Core.LastIncItemAmount / 100f}";
        }
    }

    private void ButtonPaste_Click(object sender, RoutedEventArgs e)
    {
        TxtDescription.Text = Core.LastIncItemRubric;
        TxtAmount.Text = (Core.LastIncItemAmount / 100f).ToString("0.00", CultureInfo.CurrentCulture);
    }

    private void FromDateButton_Click(object sender, RoutedEventArgs e)
    {
        DateChooserWindow w = new DateChooserWindow() {Owner = this};
        if (w.ShowDialog() == true)
        {
            DateTime d = w.SelectedDate;
            FromDateTextBlock.Tag = d;
            FromDateTextBlock.Text = d.ToShortDateString();
            Ink.CoversPeriodFromDate = d;
            ShowDayCount();
        }
    }

    private void ToDateButton_Click(object sender, RoutedEventArgs e)
    {
        DateChooserWindow w = new DateChooserWindow() {Owner = this};
        if (w.ShowDialog() == true)
        {
            DateTime d = w.SelectedDate;
            ToDateTextBlock.Tag = d;
            ToDateTextBlock.Text = d.ToShortDateString();
            Ink.CoversPeriodToDate = d;
            ShowDayCount();
        }
    }

}
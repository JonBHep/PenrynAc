using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PenrynAc;

public partial class ExpenditureItemWindow
{
    public ExpenditureItemWindow(ExpenditureItem item)
    {
        InitializeComponent();
        Z = new ExpenditureItem(item.Specification);
    }

    private readonly RadioButton[] _radio = new RadioButton[5];

    // public bool ParamNewItem { get; init; }
    private ExpenditureItem Z { get; set; }
    public string ExpenditureItemSpec => Z.Specification;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LblAmountInterpret.Text = string.Empty;
        _radio[0] = Radio1;
        _radio[1] = Radio2;
        _radio[2] = Radio3;
        _radio[3] = Radio4;
        _radio[4] = Radio5;
        _radio[0].Tag = Core.ExpenditureCategoryConstant.AllowableAgentFees;
        _radio[1].Tag = Core.ExpenditureCategoryConstant.AllowableRatesInsurance;
        _radio[2].Tag = Core.ExpenditureCategoryConstant.AllowableRepairsMaintenanceRenewals;
        _radio[3].Tag = Core.ExpenditureCategoryConstant.NonAllowableSettingUpCosts;
        _radio[4].Tag = Core.ExpenditureCategoryConstant.NonAllowableOther;

        for (var r = 0; r < 5; r++)
        {
            _radio[r].Content = Core.ExpenditureCategoryCaption((Core.ExpenditureCategoryConstant) _radio[r].Tag);
        }

        // if (ParamNewItem)
        // {
        //     Z = new ExpenditureItem(taxPt: DateTime.Today, allocTaxYr: Core.TaxYearFromDate(DateTime.Today)
        //         , description: string.Empty, penceValue: 0, categ: Core.ExpenditureCategoryConstant.Unknown);
        // }

        PayDateTextBlock.Tag = Z.PayDate;
        PayDateTextBlock.Text = Z.PayDate.ToShortDateString();

        int targetCat = 0;
        for (int z = 0; z < 5; z++)
        {
            if ((Core.ExpenditureCategoryConstant) _radio[z].Tag == Z.Category)
            {
                targetCat = z;
            }
        }

        _radio[targetCat].IsChecked = true;

        TxtAllocYear.Text = Z.AllocatedTaxYear.ToString(CultureInfo.CurrentCulture);
        TxtAmount.Text = (Z.AmountPence / (decimal) 100).ToString("0.00", CultureInfo.CurrentCulture);
        TxtRubric.Text = Z.Rubric;
    }

    private void TxtAmount_TextChanged(object sender, TextChangedEventArgs e)
    {
        // identical sub in income dialogue
        string qs = TxtAmount.Text.Trim();
        if (string.IsNullOrWhiteSpace(qs))
        {
            LblAmountInterpret.Text = "Amount is missing";
            LblAmountInterpret.Foreground = Brushes.Red;
            Z.AmountPence = 0;
        }
        else
        {
            if (decimal.TryParse(qs, out decimal sAmount))
            {
                LblAmountInterpret.Text = sAmount.ToString("C", CultureInfo.CurrentCulture);
                if (sAmount <= 0)
                {
                    LblAmountInterpret.Foreground = Brushes.Red;
                    Z.AmountPence = Convert.ToInt32(sAmount * 100); // allowed but will be queried
                }
                else
                {
                    LblAmountInterpret.Foreground = Brushes.Blue;
                    Z.AmountPence = Convert.ToInt32(sAmount * 100); // allowed but will be queried
                }
            }
            else
            {
                LblAmountInterpret.Text = "Amount is not a valid number";
                LblAmountInterpret.Foreground = Brushes.Red;
                Z.AmountPence = 0;
            }
        }
    }

    private void TxtRubric_TextChanged(object sender, TextChangedEventArgs e)
    {
        Z.Rubric = TxtRubric.Text.Trim();
    }

    private void Radio_Checked(object sender, RoutedEventArgs e)
    {
        RadioButton r = (RadioButton) sender;
        Z.Category = (Core.ExpenditureCategoryConstant) r.Tag;
    }

    private void TxtAllocYear_TextChanged(object sender, TextChangedEventArgs e)
    {
        string tx = TxtAllocYear.Text.Trim();
        if (string.IsNullOrWhiteSpace(tx))
        {
            Z.AllocatedTaxYear = 0;
        }
        else
        {
            if (!int.TryParse(tx, out int ty)) ty = 0;
            Z.AllocatedTaxYear = ty;
        }
    }

    private void BtnOK_Click(object sender, RoutedEventArgs e)
    {
        if (Z.AmountPence == 0)
        {
            MessageBox.Show("Error in amount", "Lettings", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            return;
        }

        if (Z.AmountPence < 0)
        {
            if (MessageBox.Show("Amount is negative!\nAre you really entering a rebate?", "Lettings"
                    , MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.No) return;
        }

        if (string.IsNullOrWhiteSpace(Z.Rubric))
        {
            MessageBox.Show("You must enter a description", "Lettings", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            return;
        }

        if (Z.PayDate > DateTime.Today)
        {
            MessageBox.Show("Date is in the future", "Lettings", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            return;
        }

        if (Z.PayDate < new DateTime(2005, 1, 1))
        {
            MessageBox.Show("Date is too long ago", "Lettings", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            return;
        }

        if ((Z.AllocatedTaxYear < 2005) || (Z.AllocatedTaxYear > (DateTime.Today.Year + 1)))
        {
            MessageBox.Show("Tax year is invalid", "Lettings", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            return;
        }

        if (Z.Category == Core.ExpenditureCategoryConstant.Unknown)
        {
            MessageBox.Show("Category must be selected", "Lettings", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            return;
        }

        if (!(Z.AllocatedTaxYear == Core.TaxYearFromDate(Z.PayDate)))
        {
            var msg = "Match tax year to payment date?\n\n";
            msg += "Payment date: " + Z.PayDate.ToShortDateString();
            msg += "\nTax year: " + Z.TaxYearString;
            msg += "\n\nClick YES to change tax year to " +
                   Core.TaxYearFromDate(Z.PayDate).ToString(CultureInfo.CurrentCulture);
            msg += "\nClick NO to keep tax year as " + Z.TaxYearString;
            if (MessageBox.Show(msg, "Lettings", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes)
            {
                Z.AllocatedTaxYear = Core.TaxYearFromDate(Z.PayDate);
            }
        }

        Core.LastExpItemRubric = TxtRubric.Text;
        Core.LastExpItemAmount = Z.AmountPence;
        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        PayDateButton.Focus();
        if (string.IsNullOrWhiteSpace(Core.LastExpItemRubric))
        {
            ButtonPaste.IsEnabled = false;
        }
        else
        {
            ButtonPaste.IsEnabled = true;
            TextblockLastDescription.Text = $"{Core.LastExpItemRubric} £{Core.LastExpItemAmount / 100f}";
        }
    }

    private void ButtonPaste_Click(object sender, RoutedEventArgs e)
    {
        TxtRubric.Text = Core.LastExpItemRubric;
        TxtAmount.Text = (Core.LastExpItemAmount / 100f).ToString("0.00", CultureInfo.CurrentCulture);
    }

    private void PayDateButton_Click(object sender, RoutedEventArgs e)
    {
        DateChooserWindow w = new DateChooserWindow() {Owner = this};
        if (w.ShowDialog() == true)
        {
            DateTime d = w.SelectedDate;
            PayDateTextBlock.Tag = d;
            PayDateTextBlock.Text = d.ToShortDateString();
            Z.ExceptionallySetPayDate(d);
        }
    }
}
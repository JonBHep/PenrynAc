using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PenrynAc;

public partial class AnnualSummaryWindow
{
    public enum YearType
    {
        TaxYearAccrual
        , TaxYearCashBasis
        , CalendarYearAccrual
        , CalendarYearCash
    };

    private YearType _selectedYearType = YearType.TaxYearCashBasis;
    private int _selectedYear;

    private int _annualIncomeAll;
    private int _annualIncomeL1;
    private int _annualIncomeL2;
    private int _annualIncomeFurnishedAll;
    private int _annualIncomeFurnishedL1;
    private int _annualIncomeFurnishedL2;
    private readonly int[] _annualExpenditure;
    private int _annualExpenditureTotalAll;
    private int _annualExpenditureTotalL1;
    private int _annualExpenditureTotalL2;
    private int _annualExpenditureAllowableAll;
    private int _annualExpenditureAllowableL1;
    private int _annualExpenditureAllowableL2;

    private readonly System.Collections.ObjectModel.ObservableCollection<Fourply> _summaryLines = new();

    private readonly System.Collections.ObjectModel.ObservableCollection<IncomeLine> _incomeLines = new();

    private readonly System.Collections.ObjectModel.ObservableCollection<ExpenditureLine> _expenditureLines = new();

    public AnnualSummaryWindow()
    {
        InitializeComponent();
        TextblockAddress.Text = Core.Accounts.PropertyAddress;
        LvwSummary.ItemsSource = _summaryLines;
        LvwIncome.ItemsSource = _incomeLines;
        LvwExpenditure.ItemsSource = _expenditureLines;
        _annualExpenditure = new int[Core.TopExpenditureCategory + 1];
    }

    private class Fourply
    {
        public string First { get; set; }
        public string Second { get; set; }
        public string Third { get; set; }
        public string Fourth { get; set; }
    }

    private class IncomeLine
    {
        public IncomeLine(IncomeItem item, int countedSum, int countedDays)
        {
            Received = item.DateReceived.ToShortDateString();
            Sum = Core.MoneyString(item.AmountPence);
            SumInYear =Core.MoneyString(countedSum);
            Description =item.Rubric;
            CoversFrom =item.CoversPeriodFromDate.ToShortDateString();
            CoversTo =item.CoversPeriodToDate.ToShortDateString();
            Days =item.DaysCovered.ToString();
            DaysInYear =countedDays.ToString();
            Furnished =item.Furnished ? "Yes":"No";
        }
        public string Received { get; set; }
        public string Sum { get; set; }
        public string SumInYear { get; set; }
        public string Description { get; set; }
        public string CoversFrom { get; set; }
        public string CoversTo { get; set; }
        public string Days { get; set; }
        public string DaysInYear { get; set; }
        public string Furnished { get; set; }
    }

    private class ExpenditureLine
    {
        public ExpenditureLine(ExpenditureItem item)
        {
            Date = item.PayDate.ToShortDateString();
            Sum = Core.MoneyString(item.AmountPence);
            TaxYear = item.TaxYearString;
            Category=Core.ExpenditureCategoryCaption(item.Category);
            Description = item.Rubric;
            Allowable = Core.ExpenditureReducingTaxableProfit(item.Category) ? "Yes" : "No";
        }
        
        public string Date { get; set; }
        public string Sum { get; set; }
        public string TaxYear { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Allowable { get; set; }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var scrX = SystemParameters.PrimaryScreenWidth;
        var scrY = SystemParameters.PrimaryScreenHeight;
        var winX = scrX * .98;
        var winY = scrY * .94;
        var xm = (scrX - winX) / 2;
        var ym = (scrY - winY) / 4;
        Width = winX;
        Height = winY;
        Left = xm;
        Top = ym;

        CboYearType.Items.Clear();
        var it = new ComboBoxItem()
            {Tag = YearType.TaxYearAccrual, Content = "Tax year - traditional accounting"};
        CboYearType.Items.Add(it);
        it = new ComboBoxItem() {Tag = YearType.TaxYearCashBasis, Content = "Tax year - cash basis accounting"};
        CboYearType.Items.Add(it);
        it = new ComboBoxItem()
            {Tag = YearType.CalendarYearAccrual, Content = "Calendar year - traditional accounting"};
        CboYearType.Items.Add(it);
        it = new ComboBoxItem() {Tag = YearType.CalendarYearCash, Content = "Calendar year - cash basis accounting"};
        CboYearType.Items.Add(it);

        CboYearType.SelectedIndex = 1;
        TextblockAddress.Text = Core.Accounts.PropertyAddress;
        DateConfirmationTextBlock.Text = string.Empty;
        OwnershipChangeAlertTextBlock.Visibility = Visibility.Hidden;
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void CboYearType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CboYearType.SelectedItem == null) return;
        ComboBoxItem cbi = (ComboBoxItem) CboYearType.SelectedItem;
        _selectedYearType = (YearType) cbi.Tag;

        CboYear.Items.Clear();
        CboYear.Items.Add(new ListingItem(0, "Select year"));
        if ((_selectedYearType == YearType.TaxYearAccrual) || (_selectedYearType == YearType.TaxYearCashBasis))
        {
            for (int y = DateTime.Today.Year; y > 2004; y--)
            {
                ListingItem jit = new ListingItem(y
                    , y.ToString(CultureInfo.CurrentCulture) + "-" + (y + 1).ToString(CultureInfo.CurrentCulture));
                CboYear.Items.Add(jit);
            }
        }
        else
        {
            for (int y = DateTime.Today.Year; y > 2004; y--)
            {
                ListingItem jit = new ListingItem(y, y.ToString(CultureInfo.CurrentCulture));
                CboYear.Items.Add(jit);
            }
        }

        CboYear.SelectedIndex = 0;
        BtnSave.IsEnabled = BtnShow.IsEnabled = false;
    }

    private void CboYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        BtnSave.IsEnabled = BtnShow.IsEnabled = false;
        ListingItem? jli = CboYear.SelectedItem as ListingItem?;
        if (jli.HasValue == false)
        {
            return;
        }

        if (jli.Value.ItemIndex == 0)
        {
            return;
        }

        _expenditureLines.Clear();
        _incomeLines.Clear();
        _summaryLines.Clear();
        _selectedYear = jli.Value.ItemIndex;
        TabCtlTabs.SelectedIndex = 0; // this selects the Summary tab
        BtnShow.IsEnabled = true;
    }

    private void RefreshSummary()
    {
        Tuple<DateTime, DateTime> YearBounds = FirstAndLast();

        Core.ExpenditureCategoryConstant k;
        int sum;
        _summaryLines.Clear();

        Fourply it = new Fourply();
        _summaryLines.Add(it);

        string periodstring
            = $"Year from {YearBounds.Item1.ToShortDateString()} to {YearBounds.Item2.ToShortDateString()}";
        DateConfirmationTextBlock.Text = periodstring;

        it = new Fourply
        {
            First = periodstring.ToUpperInvariant()
        };
        if (Core.Accounts.LandlordShares.SharingInYearStarting(YearBounds.Item1))
        {
            SharedOwnershipTextBlock.Visibility = Visibility.Visible;
            it.Third = "JBH";
            it.Fourth = "RJS";
        }
        else
        {
            SharedOwnershipTextBlock.Visibility = Visibility.Collapsed;
        }

        _summaryLines.Add(it);

        if (Core.Accounts.LandlordShares.HasChangeInPeriod(YearBounds.Item1, YearBounds.Item2))
        {
            OwnershipChangeAlertTextBlock.Visibility = Visibility.Visible;
            it = new Fourply() {First = "THERE WAS A CHANGE IN LANDLORD SHARES DURING THIS PERIOD"};
            _summaryLines.Add(it);
        }
        else
        {
            OwnershipChangeAlertTextBlock.Visibility = Visibility.Collapsed;
        }

        it = new Fourply();
        _summaryLines.Add(it);

        it = MakeSummaryLine(_annualIncomeAll, _annualIncomeL1, _annualIncomeL2, "TOTAL INCOME");
        _summaryLines.Add(it);

        it = MakeSummaryLine(_annualIncomeFurnishedAll, _annualIncomeFurnishedL1, _annualIncomeFurnishedL2
            , "Income from furnished letting");
        _summaryLines.Add(it);

        it = new Fourply();
        _summaryLines.Add(it);

        for (int j = 0; j <= Core.TopExpenditureCategory; j++)
        {
            k = (Core.ExpenditureCategoryConstant) j;
            // deductibles only - excludes setting up costs
            if (Core.ExpenditureReducingTaxableProfit(k))
            {
                sum = _annualExpenditure[j];
                if (sum > 0) // don't include a line with no expenditure
                {
                    it = MakeSummaryLine(sum, 0, 0, Core.ExpenditureCategoryCaption(k));
                    _summaryLines.Add(it);
                }
            }
        }

        it = MakeSummaryLine(_annualExpenditureAllowableAll, _annualExpenditureAllowableL1
            , _annualExpenditureAllowableL2, "ALLOWABLE RUNNING COSTS");
        _summaryLines.Add(it);

        for (int j = 0; j <= Core.TopExpenditureCategory; j++)
        {
            k = (Core.ExpenditureCategoryConstant) j;
            // deductibles only - excludes setting up costs
            if (!Core.ExpenditureReducingTaxableProfit(k))
            {
                sum = _annualExpenditure[j];
                if (sum > 0) // don't include a line with no expenditure
                {
                    it = MakeSummaryLine(sum, 0, 0
                        , Core.ExpenditureCategoryCaption(
                            k)); // No need to assign this line to multiple landlords, it just shows how the total allowable expenses are built up.
                    _summaryLines.Add(it);
                }
            }
        }

        it = MakeSummaryLine(_annualExpenditureTotalAll, _annualExpenditureTotalL1, _annualExpenditureTotalL2
            , "TOTAL EXPENDITURE");
        _summaryLines.Add(it);

        it = new Fourply();
        _summaryLines.Add(it);

        it = MakeSummaryLine(_annualIncomeAll - _annualExpenditureTotalAll, _annualIncomeL1 - _annualExpenditureTotalL1
            , _annualIncomeL2 - _annualExpenditureTotalL2, "INCOME LESS TOTAL EXPENDITURE");
        _summaryLines.Add(it);

        it = new Fourply();
        _summaryLines.Add(it);

        it = new Fourply
        {
            First = "TAX CALCULATION"
        };
        _summaryLines.Add(it);

        it = MakeSummaryLine(_annualIncomeAll, _annualIncomeL1, _annualIncomeL2, "Income");
        _summaryLines.Add(it);

        it = MakeSummaryLine(_annualExpenditureAllowableAll, _annualExpenditureAllowableL1
            , _annualExpenditureAllowableL2, "Less allowable running costs");
        _summaryLines.Add(it);

        int sumAll = Convert.ToInt32(_annualIncomeFurnishedAll / (decimal) 10);
        int sumL1 = Convert.ToInt32(_annualIncomeFurnishedL1 / (decimal) 10);
        int sumL2 = Convert.ToInt32(_annualIncomeFurnishedL2 / (decimal) 10);
        it = MakeSummaryLine(sumAll, sumL1, sumL2, "Less 10% of furnished letting income allowed for wear and tear");
        _summaryLines.Add(it);

        sumAll = _annualIncomeAll - sumAll - _annualExpenditureAllowableAll;
        sumL1 = _annualIncomeL1 - sumL1 - _annualExpenditureAllowableL1;
        sumL2 = _annualIncomeL2 - sumL2 - _annualExpenditureAllowableL2;
        it = MakeSummaryLine(sumAll, sumL1, sumL2, "NET PROFIT for tax purposes");
        _summaryLines.Add(it);




    }

    private Tuple<DateTime, DateTime> FirstAndLast()
    {
        DateTime d_one;
        DateTime d_two;
        if ((_selectedYearType == YearType.TaxYearAccrual) || (_selectedYearType == YearType.TaxYearCashBasis))
        {
            d_one = new DateTime(_selectedYear, 4, 6);
            d_two = new DateTime(_selectedYear + 1, 4, 5);
        }
        else
        {
            d_one = new DateTime(_selectedYear, 1, 1);
            d_two = new DateTime(_selectedYear, 12, 31);
        }

        return new Tuple<DateTime, DateTime>(d_one, d_two);
    }

    private static Fourply MakeSummaryLine(int Overall, int FirstShare, int SecondShare, string Caption)
    {
        Fourply rv = new Fourply
        {
            First = Caption, Second = Core.MoneyString(Overall)
        };
        if (SecondShare == 0)
        {
            // single landlord, so don't break down the sum
            rv.Third = string.Empty;
            rv.Fourth = string.Empty;
        }
        else
        {
            // two landlords, so show proportionate shares
            rv.Third = Core.MoneyString(FirstShare);
            rv.Fourth = Core.MoneyString(SecondShare);
        }

        return rv;
    }

    private void BtnShow_Click(object sender, RoutedEventArgs e)
    {
        RefreshExpenditureList();
        RefreshIncomeList();
        RefreshSummary();
        BtnSave.IsEnabled = true;
    }

    private void RefreshExpenditureList()
    {
        _expenditureLines.Clear();
        bool flag;
        _annualExpenditureTotalAll = 0;
        _annualExpenditureTotalL1 = 0;
        _annualExpenditureTotalL2 = 0;
        _annualExpenditureAllowableAll = 0;
        _annualExpenditureAllowableL1 = 0;
        _annualExpenditureAllowableL2 = 0;
        for (int y = 0; y <= Core.TopExpenditureCategory; y++)
        {
            _annualExpenditure[y] = 0;
        }

        foreach (string ky in Core.Accounts.ExpenditureItems.Keys)
        {
            ExpenditureItem ei = Core.Accounts.ExpenditureItems[ky];
            flag = false;
            switch (_selectedYearType)
            {
                case YearType.TaxYearAccrual:
                {
                    flag = (ei.AllocatedTaxYear == _selectedYear);
                    break;
                }
                case YearType.TaxYearCashBasis:
                {
                    flag = (Core.TaxYearFromDate(ei.PayDate) == _selectedYear);
                    break;
                }
                case YearType.CalendarYearAccrual:
                case YearType.CalendarYearCash:
                {
                    flag = (ei.PayDate.Year == _selectedYear);
                    break;
                }
            }

            if (flag)
            {
                int etot = ei.AmountPence;
                int exp1 = Core.Accounts.LandlordShares.FirstShare(etot, ei.PayDate);
                int exp2 = Core.Accounts.LandlordShares.SecondShare(etot, ei.PayDate);
                _annualExpenditureTotalAll += etot;
                _annualExpenditureTotalL1 += exp1;
                _annualExpenditureTotalL2 += exp2;
                _annualExpenditure[(int) ei.Category] += etot;
                var lyne = new ExpenditureLine(ei);
                if (Core.ExpenditureReducingTaxableProfit(ei.Category))
                {
                    _annualExpenditureAllowableAll += etot;
                    _annualExpenditureAllowableL1 += exp1;
                    _annualExpenditureAllowableL2 += exp2;
                }
                _expenditureLines.Add(lyne);
            }
        }
    }

    private void RefreshIncomeList()
    {
        _incomeLines.Clear();
        bool flag;
        int countedAmount = 0;
        int countedDays = 0;
        SortedDictionary<string, int> incDic = new SortedDictionary<string, int>();
        _annualIncomeAll = 0;
        _annualIncomeL1 = 0;
        _annualIncomeL2 = 0;
        _annualIncomeFurnishedAll = 0;
        _annualIncomeFurnishedL1 = 0;
        _annualIncomeFurnishedL2 = 0;

        foreach (string ky in Core.Accounts.IncomeItems.Keys)
        {
            IncomeItem ii = Core.Accounts.IncomeItems[ky];
            flag = ii.HitsYear(_selectedYear, _selectedYearType);
            if (flag)
            {
                switch (_selectedYearType)
                {
                    case YearType.TaxYearAccrual:
                    {
                        countedAmount = ii.AmountPenceInTaxYear(_selectedYear);
                        countedDays = ii.DaysCoveredInTaxYear(_selectedYear);
                        break;
                    }
                    case YearType.TaxYearCashBasis:
                    {
                        countedAmount = ii.AmountPence;
                        countedDays = 0;
                        break;
                    }
                    case YearType.CalendarYearAccrual:
                    {
                        countedAmount = ii.AmountPenceInCalendarYear(_selectedYear);
                        countedDays = ii.DaysCoveredInCalendarYear(_selectedYear);
                        break;
                    }
                    case YearType.CalendarYearCash:
                    {
                        countedAmount = ii.AmountPence;
                        countedDays = 0;
                        break;
                    }
                }

                int inc1 = Core.Accounts.LandlordShares.FirstShare(countedAmount, ii.DateReceived);
                int inc2 = Core.Accounts.LandlordShares.SecondShare(countedAmount, ii.DateReceived);
                _annualIncomeAll += countedAmount;
                _annualIncomeL1 += inc1;
                _annualIncomeL2 += inc2;

                if (ii.Furnished)
                {
                    _annualIncomeFurnishedAll += countedAmount;
                    _annualIncomeFurnishedL1 += inc1;
                    _annualIncomeFurnishedL2 += inc2;
                }

                if (incDic.ContainsKey(ii.Rubric))
                {
                    incDic[ii.Rubric] += countedAmount;
                }
                else
                {
                    incDic.Add(ii.Rubric, countedAmount);
                }

                var lyne = new IncomeLine(ii,countedAmount, countedDays);
                
                if (ii.Furnished)
                {
                    lyne.Furnished = "Yes";
                }
                else
                {
                    lyne.Furnished = "No";
                }

                lyne.CoversFrom = ii.CoversPeriodFromDate.ToShortDateString();
                lyne.CoversTo = ii.CoversPeriodToDate.ToShortDateString();
                lyne.Days = ii.DaysCovered.ToString(CultureInfo.CurrentCulture);
                lyne.DaysInYear = (countedDays == 0) ? string.Empty : countedDays.ToString(CultureInfo.CurrentCulture);
                _incomeLines.Add(lyne);
            }
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {

        // TODO Neaten this output document (maybe use XPS as seen in Crux project)
        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
        {
            DefaultExt = "txt", AddExtension = true, CheckPathExists = true, Filter = "Text files (*.txt)|*.txt"
            , OverwritePrompt = true, Title = "Save details in text file", ValidateNames = true
        };
        bool? dialogok = dlg.ShowDialog();
        if ((dialogok.HasValue) && (dialogok.Value == true))
        {
            string filepath = dlg.FileName;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filepath))
            {
                sw.WriteLine("LETTING ACCOUNTS");
                sw.WriteLine($"{TextblockTitle.Text}: {TextblockAddress.Text}");
                sw.WriteLine($"{CboYearType.Text} {CboYear.Text}");
                sw.WriteLine("");
                sw.WriteLine("\nINCOME");
                sw.WriteLine("");
                foreach (IncomeLine i in _incomeLines)
                {
                    string oput = $"{i.Received} {i.Sum} {i.Description} for period {i.CoversFrom} to {i.CoversTo}";
                    if (i.Sum != i.SumInYear)
                    {
                        oput += $" ({i.SumInYear} in this tax year)";
                    }

                    sw.WriteLine(oput);
                }

                sw.WriteLine("");
                sw.WriteLine("EXPENDITURE");
                sw.WriteLine("");
                foreach (ExpenditureLine ei in _expenditureLines)
                {
                    string oput = $"{ei.Date} {ei.Sum} {ei.Description} category: {ei.Category}";
                    if (ei.Allowable == "Yes")
                    {
                        oput += " (Allowable)";
                    }
                    else
                    {
                        oput += " (Not allowable)";
                    }

                    sw.WriteLine(oput);
                }

                sw.WriteLine("");
                sw.WriteLine("SUMMARY");
                sw.WriteLine("");


                foreach (Fourply fp in _summaryLines)
                {
                    if (string.IsNullOrWhiteSpace(fp.First))
                    {
                        sw.WriteLine("");
                    }
                    else if (string.IsNullOrWhiteSpace(fp.Third))
                    {
                        // single landlord, so don't break down the sum
                        string oput = $"{fp.First} {fp.Second}";
                        sw.WriteLine(oput);
                    }
                    else
                    {
                        // two landlords, so show proportionate shares
                        string oput = $"{fp.First} {fp.Second}  (JBH {fp.Third}; RJS {fp.Fourth})";
                        sw.WriteLine(oput);
                    }
                }

                sw.WriteLine("\nEND");
            }
        }
    }
}
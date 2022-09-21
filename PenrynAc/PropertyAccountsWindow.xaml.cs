using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PenrynAc;

public partial class PropertyAccountsWindow
{
    private readonly DateTime _backTimeLimit = DateTime.Today.AddYears(-2);

        public PropertyAccountsWindow()
        {
            InitializeComponent();
            TextblockAddress.Text = PropertyAccounts.Instance.PropertyAddress;
            LvwSummary.ItemsSource = _summaryLines;
            LvwIncome.ItemsSource = _incomeLines;
            LvwExpenditure.ItemsSource = _expenditureLines;
            LvwCommon.ItemsSource = _commonLines;
            RefreshSummary();
            //RefreshIncomeItems(); // These two Refreshes are triggered by CheckBox.IsChecked setting on their respective tabs, so don't need to be triggered here
            //RefreshExpenditureItems();
            RefreshCommonItems();
        }

        private readonly ObservableCollection<Fourply> _summaryLines = new ObservableCollection<Fourply>();
        private readonly ObservableCollection<IncomeLine> _incomeLines = new ObservableCollection<IncomeLine>();
        private readonly ObservableCollection<ExpenditureLine> _expenditureLines = new ObservableCollection<ExpenditureLine>();
        private readonly ObservableCollection<CommonLine> _commonLines = new ObservableCollection<CommonLine>();

        private class Fourply
        {
            public Fourply()
            {
                First = Second = Third = Fourth = string.Empty;
            }
            public string First { get; set; }
            public string Second { get; set; }
            public string Third { get; set; }
            public string Fourth { get; set; }
        }

        private class IncomeLine
        {
            public IncomeLine()
            {
                Received = Sum = FirstShare = SecondShare
                    = Description = CoversFrom = CoversTo = Days = Furnished = Key = string.Empty;
            }
            public string Received { get; set; }
            public string Sum { get; set; }
            public string FirstShare { get; set; }
            public string SecondShare { get; set; }
            public string Description { get; set; }
            public string CoversFrom { get; set; }
            public string CoversTo { get; set; }
            public string Days { get; set; }
            public string Furnished { get; set; }
            public string Key { get; set; }
        }

        private class ExpenditureLine
        {
            public ExpenditureLine()
            {
                Date = Sum = FirstShare = SecondShare = TaxYear = Category = Description = Key = string.Empty;
            }
            public string Date { get; set; }
            public string Sum { get; set; }
            public string FirstShare { get; set; }
            public string SecondShare { get; set; }
            public string TaxYear { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string Key { get; set; }
        }

        private class CommonLine : IComparable<CommonLine>
        {
            public CommonLine()
            {
                Date = Amount = Description = string.Empty;
                When=DateTime.MinValue;
                Tint=Brushes.Black;
            }
            public string Date { get; set; }
            public DateTime When { get; set; }
            public string Amount { get; set; }
            public string Description { get; set; }
            public Brush Tint { get; set; }

            int IComparable<CommonLine>.CompareTo(CommonLine? other)
            {
                if (other is null) return 0;
                var d = When.CompareTo(other.When);
                if (d == 0) { d =string.Compare( other.Amount,Amount, true, CultureInfo.CurrentCulture); }
                return d;

            }
        }

        private void RefreshSummary()
        {
            // blank line
            var myline = new Fourply();
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "Purchased",
                Second = PropertyAccounts.Instance.PropertyPurchaseDate.ToLongDateString(),
                Third = "",
                Fourth = ""
            };
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "Years since purchased",
                Second = PropertyAccounts.Instance.YearsRun.ToString("0.00", CultureInfo.CurrentCulture)
            };
            if (PropertyAccounts.Instance.LandlordShares.NumberOfPhases > 1)
            {
                myline.Third = "First share";
                myline.Fourth = "Second share";
            }
            _summaryLines.Add(myline);

            // blank line
            myline = new Fourply();
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "Purchase cost"
            };
            var x = PropertyAccounts.Instance.PropertyPurchaseCost;
            var xf = PropertyAccounts.Instance.LandlordShares.FirstShare(x, PropertyAccounts.Instance.PropertyPurchaseDate);
            var xs = PropertyAccounts.Instance.LandlordShares.SecondShare(x, PropertyAccounts.Instance.PropertyPurchaseDate);
            myline.Second = Core.MoneyString(x);
            if (xs > 0)
            {
                myline.Third = Core.MoneyString(xf);
                myline.Fourth = Core.MoneyString(xs);
            }
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "Setting up costs"
            };
            x = PropertyAccounts.Instance.AggregateCostSettingUp;
            xf = PropertyAccounts.Instance.LandlordShares.FirstShare(x, PropertyAccounts.Instance.PropertyPurchaseDate);
            xs = PropertyAccounts.Instance.LandlordShares.SecondShare(x, PropertyAccounts.Instance.PropertyPurchaseDate);
            myline.Second = Core.MoneyString(x);
            if (xs > 0)
            {
                myline.Third = Core.MoneyString(xf);
                myline.Fourth = Core.MoneyString(xs);
            }
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "Subsequent costs"
            };
            x = PropertyAccounts.Instance.AggregateCostSubsequent(out var l1, out var l2);
            myline.Second = Core.MoneyString(x);
            if (l2 > 0)
            {
                myline.Third = Core.MoneyString(l1);
                myline.Fourth = Core.MoneyString(l2);
            }
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "TOTAL COSTS"
            };
            x = PropertyAccounts.Instance.AggregateAllTimeCostTotal(out var a1, out var a2);
            myline.Second = Core.MoneyString(x);
            if (a2 > 0)
            {
                myline.Third = Core.MoneyString(a1);
                myline.Fourth = Core.MoneyString(a2);
            }
            _summaryLines.Add(myline);

            // blank line
            myline = new Fourply();
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "TOTAL INCOME"
            };
            x = PropertyAccounts.Instance.AggregateAllTimeIncomeTotal(out var b1, out var b2);
            myline.Second = Core.MoneyString(x);
            if (b2 > 0)
            {
                myline.Third = Core.MoneyString(b1);
                myline.Fourth = Core.MoneyString(b2);
            }
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "As percentage of total cost"
            };
            var pc = (double)PropertyAccounts.Instance.AggregateAllTimeIncomeTotal(out _, out _) / PropertyAccounts.Instance.AggregateAllTimeCostTotal(out _ , out _);
            pc *= 100;
            myline.Second = pc.ToString("0.00", CultureInfo.CurrentCulture) + "%";
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "Average annual income"
            };
            x = PropertyAccounts.Instance.AverageAnnualIncome(out var r1, out var r2);
            myline.Second = Core.MoneyString(x);
            if (r2 > 0)
            {
                myline.Third = Core.MoneyString(r1);
                myline.Fourth = Core.MoneyString(r2);
            }
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "As percentage of total cost"
            };
            pc = (double)PropertyAccounts.Instance.AverageAnnualIncome(out _, out _) / PropertyAccounts.Instance.AggregateAllTimeCostTotal(out _, out _);
            pc *= 100;
            myline.Second = pc.ToString("0.00", CultureInfo.CurrentCulture) + "%";
            _summaryLines.Add(myline);

        }

        private void RefreshIncomeItems()
        {
            _incomeLines.Clear();
            foreach (var k in PropertyAccounts.Instance.IncomeItems.Keys)
            {
                var i = PropertyAccounts.Instance.IncomeItems[k];
                if ((CheckBoxUnticked(DateLimitIncCheckBox)) || (i.CoversPeriodFromDate > _backTimeLimit))
                {
                    IncomeLine l = new IncomeLine
                    {
                        Key = k,
                        CoversFrom = i.CoversPeriodFromDate.ToShortDateString(),
                        CoversTo = i.CoversPeriodToDate.ToShortDateString(),
                        Days = i.DaysCovered.ToString( CultureInfo.CurrentCulture),
                        Description = i.Rubric,
                        FirstShare = Core.MoneyString(PropertyAccounts.Instance.LandlordShares.FirstShare(i.AmountPence, i.DateReceived)),
                        SecondShare = Core.MoneyString(PropertyAccounts.Instance.LandlordShares.SecondShare(i.AmountPence, i.DateReceived))
                        ,
                        Furnished = i.Furnished ? "Yes" : "No"
                        ,
                        Received = i.DateReceived.ToShortDateString()
                        ,
                        Sum = Core.MoneyString(i.AmountPence)
                    };
                    _incomeLines.Add(l);
                }
            }
        }

        private void RefreshExpenditureItems()
        {
            _expenditureLines.Clear();

            foreach (string k in PropertyAccounts.Instance.ExpenditureItems.Keys)
            {
                var e = PropertyAccounts.Instance.ExpenditureItems[k];
                if ((CheckBoxUnticked(DateLimitExpCheckBox)) || (e.PayDate > _backTimeLimit))
                {
                    ExpenditureLine l = new ExpenditureLine
                    {
                        Key = k,
                        Category = Core.ExpenditureCategoryCaption(e.Category),
                        Date = e.PayDate.ToShortDateString(),
                        Description = e.Rubric,
                        Sum = Core.MoneyString(e.AmountPence),
                        TaxYear = e.TaxYearString,
                        FirstShare = Core.MoneyString(PropertyAccounts.Instance.LandlordShares.FirstShare(e.AmountPence, e.PayDate)),
                        SecondShare = Core.MoneyString(PropertyAccounts.Instance.LandlordShares.SecondShare(e.AmountPence, e.PayDate))
                    };
                    _expenditureLines.Add(l);
                }
            }
        }

        private void RefreshCommonItems()
        {
            List<CommonLine> lst = new List<CommonLine>();
            foreach (string k in PropertyAccounts.Instance.ExpenditureItems.Keys)
            {
                ExpenditureItem e = PropertyAccounts.Instance.ExpenditureItems[k];
                if ((CheckBoxUnticked(DateLimitCommonCheckBox)) || (e.PayDate > _backTimeLimit))
                {
                    CommonLine cl = new CommonLine()
                    {
                        Date = e.PayDate.ToShortDateString(),
                        When = e.PayDate,
                        Amount = Core.MoneyString(e.AmountPence),
                        Description = e.Rubric,
                        Tint = Brushes.DarkRed
                    };
                    lst.Add(cl);
                }
            }
            foreach (string k in PropertyAccounts.Instance.IncomeItems.Keys)
            {
                IncomeItem i = PropertyAccounts.Instance.IncomeItems[k];
                if ((CheckBoxUnticked(DateLimitCommonCheckBox)) || (i.DateReceived > _backTimeLimit))
                {
                    CommonLine cl = new CommonLine()
                    {
                        Date = i.DateReceived.ToShortDateString(),
                        When = i.DateReceived,
                        Amount = Core.MoneyString(i.AmountPence),
                        Description = i.Rubric,
                        Tint = Brushes.DarkGreen
                    };
                    lst.Add(cl);
                }
            }
            lst.Sort();
            _commonLines.Clear();
            foreach (CommonLine cl in lst) { _commonLines.Add(cl); }
        }

        private static bool CheckBoxUnticked(CheckBox box)
        {
            bool flag = true;
            if (box.IsChecked.HasValue)
            {
                if (box.IsChecked.Value) { flag = false; }
            }
            return flag;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonEditInc_Click(object sender, RoutedEventArgs e)
        {
            if (LvwIncome.SelectedIndex == -1) return; // no item selected
            IncomeLine il = (IncomeLine)LvwIncome.SelectedItem;
            IncomeItem ii = PropertyAccounts.Instance.IncomeItems[il.Key];
            IncomeItemWindow wdw = new IncomeItemWindow(new IncomeItem(spec: ii.Specification))
            {
                Owner = this
            };
            if (wdw.ShowDialog() == true)
            {
                IncomeItem newIi = new IncomeItem(spec: wdw.IncomeItemSpec);
                if (newIi.DateReceived == ii.DateReceived) // can edit existing item
                {
                    PropertyAccounts.Instance.IncomeItems[il.Key].AmountPence = newIi.AmountPence;
                    PropertyAccounts.Instance.IncomeItems[il.Key].CoversPeriodFromDate = newIi.CoversPeriodFromDate;
                    PropertyAccounts.Instance.IncomeItems[il.Key].CoversPeriodToDate = newIi.CoversPeriodToDate;
                    PropertyAccounts.Instance.IncomeItems[il.Key].Furnished = newIi.Furnished;
                    PropertyAccounts.Instance.IncomeItems[il.Key].Rubric = newIi.Rubric;
                }
                else
                // must replace with new item as date had changed
                {
                    PropertyAccounts.Instance.IncomeItems.Remove(il.Key);
                    PropertyAccounts.Instance.IncomeItems.Add(key: PropertyAccounts.Instance.UniqueIncomeKey(newIi.DateReceived), value: newIi);
                }
                RefreshIncomeItems();
                RefreshCommonItems();
            }
        }

        private void ButtonEditExp_Click(object sender, RoutedEventArgs e)
        {
            if (LvwExpenditure.SelectedIndex == -1) return; // no item selected
            ExpenditureLine el = (ExpenditureLine)LvwExpenditure.SelectedItem;
            ExpenditureItem ei = PropertyAccounts.Instance.ExpenditureItems[el.Key];
            ExpenditureItemWindow wdw = new ExpenditureItemWindow(ei)
            {
                Owner = this
            };
            if (wdw.ShowDialog() == true)
            {
                ExpenditureItem newEi = new ExpenditureItem(spec: wdw.ExpenditureItemSpec);
                if (newEi.PayDate == ei.PayDate) //  can edit existing item
                {
                    PropertyAccounts.Instance.ExpenditureItems[el.Key].AllocatedTaxYear = newEi.AllocatedTaxYear;
                    PropertyAccounts.Instance.ExpenditureItems[el.Key].AmountPence = newEi.AmountPence;
                    PropertyAccounts.Instance.ExpenditureItems[el.Key].Category = newEi.Category;
                    PropertyAccounts.Instance.ExpenditureItems[el.Key].Rubric = newEi.Rubric;
                }
                else
                // must replace with new item as date had changed
                {
                    PropertyAccounts.Instance.ExpenditureItems.Remove(el.Key);
                    PropertyAccounts.Instance.ExpenditureItems.Add(key: PropertyAccounts.Instance.UniqueExpenditureKey(newEi.PayDate), value: newEi);
                }
                RefreshExpenditureItems();
                RefreshCommonItems();
            }
        }

        private void ButtonAddInc_Click(object sender, RoutedEventArgs e)
        {
            IncomeItem q=new  IncomeItem(whenReceived: DateTime.Today, description: string.Empty, penceValue: 0
                , periodFrom: DateTime.Today, periodTo: DateTime.Today, furnishd: false);
            IncomeItemWindow wdw = new IncomeItemWindow(q)
            {
                Owner = this
            };
            if (wdw.ShowDialog() == true)
            {
                IncomeItem newIi = new IncomeItem(spec: wdw.IncomeItemSpec);
                PropertyAccounts.Instance.IncomeItems.Add(key: PropertyAccounts.Instance.UniqueIncomeKey(newIi.DateReceived), value: newIi);
                RefreshIncomeItems();
                RefreshCommonItems();
            }
        }

        private void ButtonDeleteInc_Click(object sender, RoutedEventArgs e)
        {
            if (LvwIncome.SelectedIndex == -1) return; // no item selected
            IncomeLine il = (IncomeLine)LvwIncome.SelectedItem;
            if (MessageBox.Show(il.Received + "\n" + il.Description + "\n\nDelete this item?", "Lettings", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            PropertyAccounts.Instance.IncomeItems.Remove(il.Key);
            RefreshIncomeItems();
            RefreshCommonItems();
        }

        private void ButtonAddExp_Click(object sender, RoutedEventArgs e)
        {
            ExpenditureItem nova = new ExpenditureItem(taxPt: DateTime.Today, allocTaxYr: Core.TaxYearFromDate(DateTime.Today)
                , description: string.Empty, penceValue: 0, categ: Core.ExpenditureCategoryConstant.Unknown);
            ExpenditureItemWindow wdw = new ExpenditureItemWindow(nova)
            {
                Owner = this
            };
            if (wdw.ShowDialog() == true)
            {
                ExpenditureItem newEi = new ExpenditureItem(spec: wdw.ExpenditureItemSpec);
                PropertyAccounts.Instance.ExpenditureItems.Add(key: PropertyAccounts.Instance.UniqueExpenditureKey(newEi.PayDate), value: newEi);
                RefreshExpenditureItems();
                RefreshCommonItems();
            }
        }

        private void ButtonDeleteExp_Click(object sender, RoutedEventArgs e)
        {
            if (LvwExpenditure.SelectedIndex == -1) return; // no item selected
            ExpenditureLine el = (ExpenditureLine)LvwExpenditure.SelectedItem;
            if (MessageBox.Show(el.Date + "\n" + el.Description + "\n\nDelete this item?", "Lettings", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            PropertyAccounts.Instance.ExpenditureItems.Remove(el.Key);
            RefreshExpenditureItems();
            RefreshCommonItems();
        }

        private void BtnAnnualSummary_Click(object sender, RoutedEventArgs e)
        {
            AnnualSummaryWindow wdw = new AnnualSummaryWindow
            {
                Owner = this
            };
            wdw.ShowDialog();
        }

        private void DateLimitExpCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RefreshExpenditureItems();
        }

        private void DateLimitIncCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RefreshIncomeItems();
        }

        private void DateLimitCommonCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RefreshCommonItems();
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
        }
}
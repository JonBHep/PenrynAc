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
            textblockAddress.Text = Core.Accounts.PropertyAddress;
            lvwSummary.ItemsSource = _summaryLines;
            lvwIncome.ItemsSource = _incomeLines;
            lvwExpenditure.ItemsSource = _expenditureLines;
            lvwCommon.ItemsSource = _commonLines;
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
                Second = Core.Accounts.PropertyPurchaseDate.ToLongDateString(),
                Third = "",
                Fourth = ""
            };
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "Years since purchased",
                Second = Core.Accounts.YearsRun.ToString("0.00", CultureInfo.CurrentCulture)
            };
            if (Core.Accounts.LandlordShares.NumberOfPhases > 1)
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
            var x = Core.Accounts.PropertyPurchaseCost;
            var xf = Core.Accounts.LandlordShares.FirstShare(x, Core.Accounts.PropertyPurchaseDate);
            var xs = Core.Accounts.LandlordShares.SecondShare(x, Core.Accounts.PropertyPurchaseDate);
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
            x = Core.Accounts.AggregateCostSettingUp;
            xf = Core.Accounts.LandlordShares.FirstShare(x, Core.Accounts.PropertyPurchaseDate);
            xs = Core.Accounts.LandlordShares.SecondShare(x, Core.Accounts.PropertyPurchaseDate);
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
            x = Core.Accounts.AggregateCostSubsequent(out var l1, out var l2);
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
            x = Core.Accounts.AggregateAllTimeCostTotal(out var a1, out var a2);
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
            x = Core.Accounts.AggregateAllTimeIncomeTotal(out var b1, out var b2);
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
            var pc = (double)Core.Accounts.AggregateAllTimeIncomeTotal(out _, out _) / Core.Accounts.AggregateAllTimeCostTotal(out _ , out _);
            pc *= 100;
            myline.Second = pc.ToString("0.00", CultureInfo.CurrentCulture) + "%";
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "Average annual income"
            };
            x = Core.Accounts.AverageAnnualIncome(out var r1, out var r2);
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
            pc = (double)Core.Accounts.AverageAnnualIncome(out _, out _) / Core.Accounts.AggregateAllTimeCostTotal(out _, out _);
            pc *= 100;
            myline.Second = pc.ToString("0.00", CultureInfo.CurrentCulture) + "%";
            _summaryLines.Add(myline);

        }

        private void RefreshIncomeItems()
        {
            _incomeLines.Clear();
            foreach (var k in Core.Accounts.IncomeItems.Keys)
            {
                var i = Core.Accounts.IncomeItems[k];
                if ((CheckBoxUnticked(DateLimitIncCheckBox)) || (i.CoversPeriodFromDate > _backTimeLimit))
                {
                    IncomeLine l = new IncomeLine
                    {
                        Key = k,
                        CoversFrom = i.CoversPeriodFromDate.ToShortDateString(),
                        CoversTo = i.CoversPeriodToDate.ToShortDateString(),
                        Days = i.DaysCovered.ToString( CultureInfo.CurrentCulture),
                        Description = i.Rubric,
                        FirstShare = Core.MoneyString(Core.Accounts.LandlordShares.FirstShare(i.AmountPence, i.DateReceived)),
                        SecondShare = Core.MoneyString(Core.Accounts.LandlordShares.SecondShare(i.AmountPence, i.DateReceived))
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

            foreach (string k in Core.Accounts.ExpenditureItems.Keys)
            {
                var e = Core.Accounts.ExpenditureItems[k];
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
                        FirstShare = Core.MoneyString(Core.Accounts.LandlordShares.FirstShare(e.AmountPence, e.PayDate)),
                        SecondShare = Core.MoneyString(Core.Accounts.LandlordShares.SecondShare(e.AmountPence, e.PayDate))
                    };
                    _expenditureLines.Add(l);
                }
            }
        }

        private void RefreshCommonItems()
        {
            List<CommonLine> lst = new List<CommonLine>();
            foreach (string k in Core.Accounts.ExpenditureItems.Keys)
            {
                ExpenditureItem e = Core.Accounts.ExpenditureItems[k];
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
            foreach (string k in Core.Accounts.IncomeItems.Keys)
            {
                IncomeItem i = Core.Accounts.IncomeItems[k];
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
            if (lvwIncome.SelectedIndex == -1) return; // no item selected
            IncomeLine il = (IncomeLine)lvwIncome.SelectedItem;
            IncomeItem ii = Core.Accounts.IncomeItems[il.Key];
            IncomeItemWindow wdw = new IncomeItemWindow
            {
                Ink = new IncomeItem(spec: ii.Specification),
                Owner = this,
                ParamNewItem = false
            };
            if (wdw.ShowDialog() == true)
            {
                IncomeItem newIi = new IncomeItem(spec: wdw.Ink.Specification);
                if (newIi.DateReceived == ii.DateReceived) // can edit existing item
                {
                    Core.Accounts.IncomeItems[il.Key].AmountPence = newIi.AmountPence;
                    Core.Accounts.IncomeItems[il.Key].CoversPeriodFromDate = newIi.CoversPeriodFromDate;
                    Core.Accounts.IncomeItems[il.Key].CoversPeriodToDate = newIi.CoversPeriodToDate;
                    Core.Accounts.IncomeItems[il.Key].Furnished = newIi.Furnished;
                    Core.Accounts.IncomeItems[il.Key].Rubric = newIi.Rubric;
                }
                else
                // must replace with new item as date had changed
                {
                    Core.Accounts.IncomeItems.Remove(il.Key);
                    Core.Accounts.IncomeItems.Add(key: Core.Accounts.UniqueIncomeKey(newIi.DateReceived), value: newIi);
                }
                RefreshIncomeItems();
                RefreshCommonItems();
            }
        }

        private void ButtonEditExp_Click(object sender, RoutedEventArgs e)
        {
            if (lvwExpenditure.SelectedIndex == -1) return; // no item selected
            ExpenditureLine el = (ExpenditureLine)lvwExpenditure.SelectedItem;
            ExpenditureItem ei = Core.Accounts.ExpenditureItems[el.Key];
            ExpenditureItemWindow wdw = new ExpenditureItemWindow
            {
                Z = new ExpenditureItem(spec: ei.Specification),
                ParamNewItem = false,
                Owner = this
            };
            if (wdw.ShowDialog() == true)
            {
                ExpenditureItem newEi = new ExpenditureItem(spec: wdw.Z.Specification);
                if (newEi.PayDate == ei.PayDate) //  can edit existing item
                {
                    Core.Accounts.ExpenditureItems[el.Key].AllocatedTaxYear = newEi.AllocatedTaxYear;
                    Core.Accounts.ExpenditureItems[el.Key].AmountPence = newEi.AmountPence;
                    Core.Accounts.ExpenditureItems[el.Key].Category = newEi.Category;
                    Core.Accounts.ExpenditureItems[el.Key].Rubric = newEi.Rubric;
                }
                else
                // must replace with new item as date had changed
                {
                    Core.Accounts.ExpenditureItems.Remove(el.Key);
                    Core.Accounts.ExpenditureItems.Add(key: Core.Accounts.UniqueExpenditureKey(newEi.PayDate), value: newEi);
                }
                RefreshExpenditureItems();
                RefreshCommonItems();
            }
        }

        private void ButtonAddInc_Click(object sender, RoutedEventArgs e)
        {
            IncomeItemWindow wdw = new IncomeItemWindow
            {
                ParamNewItem = true,
                Owner = this
            };
            if (wdw.ShowDialog() == true)
            {
                IncomeItem newIi = new IncomeItem(spec: wdw.Ink.Specification);
                Core.Accounts.IncomeItems.Add(key: Core.Accounts.UniqueIncomeKey(newIi.DateReceived), value: newIi);
                RefreshIncomeItems();
                RefreshCommonItems();
            }
        }

        private void ButtonDeleteInc_Click(object sender, RoutedEventArgs e)
        {
            if (lvwIncome.SelectedIndex == -1) return; // no item selected
            IncomeLine il = (IncomeLine)lvwIncome.SelectedItem;
            if (MessageBox.Show(il.Received + "\n" + il.Description + "\n\nDelete this item?", "Lettings", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            Core.Accounts.IncomeItems.Remove(il.Key);
            RefreshIncomeItems();
            RefreshCommonItems();
        }

        private void ButtonAddExp_Click(object sender, RoutedEventArgs e)
        {
            ExpenditureItemWindow wdw = new ExpenditureItemWindow
            {
                ParamNewItem = true,
                Owner = this
            };
            if (wdw.ShowDialog() == true)
            {
                ExpenditureItem newEi = new ExpenditureItem(spec: wdw.Z.Specification);
                Core.Accounts.ExpenditureItems.Add(key: Core.Accounts.UniqueExpenditureKey(newEi.PayDate), value: newEi);
                RefreshExpenditureItems();
                RefreshCommonItems();
            }
        }

        private void ButtonDeleteExp_Click(object sender, RoutedEventArgs e)
        {
            if (lvwExpenditure.SelectedIndex == -1) return; // no item selected
            ExpenditureLine el = (ExpenditureLine)lvwExpenditure.SelectedItem;
            if (MessageBox.Show(el.Date + "\n" + el.Description + "\n\nDelete this item?", "Lettings", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            Core.Accounts.ExpenditureItems.Remove(el.Key);
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
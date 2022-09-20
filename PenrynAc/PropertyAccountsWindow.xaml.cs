using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PenrynAc;

public partial class PropertyAccountsWindow : Window
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

        readonly ObservableCollection<Fourply> _summaryLines = new ObservableCollection<Fourply>();
        readonly ObservableCollection<IncomeLine> _incomeLines = new ObservableCollection<IncomeLine>();
        readonly ObservableCollection<ExpenditureLine> _expenditureLines = new ObservableCollection<ExpenditureLine>();
        readonly ObservableCollection<CommonLine> _commonLines = new ObservableCollection<CommonLine>();

        class Fourply
        {
            public string First { get; set; }
            public string Second { get; set; }
            public string Third { get; set; }
            public string Fourth { get; set; }
        }

        class IncomeLine
        {
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

        class ExpenditureLine
        {
            public string Date { get; set; }
            public string Sum { get; set; }
            public string FirstShare { get; set; }
            public string SecondShare { get; set; }
            public string TaxYear { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string Key { get; set; }
        }

        class CommonLine : IComparable<CommonLine>
        {
            public string Date { get; set; }
            public DateTime When { get; set; }
            public string Amount { get; set; }
            public string Description { get; set; }
            public Brush Tint { get; set; }

            int IComparable<CommonLine>.CompareTo(CommonLine other)
            {
                int d = this.When.CompareTo(other.When);
                if (d == 0) { d =string.Compare( other.Amount,this.Amount, true, CultureInfo.CurrentCulture); }
                return d;
            }
        }

        void RefreshSummary()
        {
            // blank line
            Fourply myline = new Fourply();
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
            int x = Core.Accounts.PropertyPurchaseCost;
            int xf = Core.Accounts.LandlordShares.FirstShare(x, Core.Accounts.PropertyPurchaseDate);
            int xs = Core.Accounts.LandlordShares.SecondShare(x, Core.Accounts.PropertyPurchaseDate);
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
            x = Core.Accounts.AggregateCostSubsequent(out int L1, out int L2);
            myline.Second = Core.MoneyString(x);
            if (L2 > 0)
            {
                myline.Third = Core.MoneyString(L1);
                myline.Fourth = Core.MoneyString(L2);
            }
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "TOTAL COSTS"
            };
            x = Core.Accounts.AggregateAllTimeCostTotal(out int A1, out int A2);
            myline.Second = Core.MoneyString(x);
            if (A2 > 0)
            {
                myline.Third = Core.MoneyString(A1);
                myline.Fourth = Core.MoneyString(A2);
            }
            _summaryLines.Add(myline);

            // blank line
            myline = new Fourply();
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "TOTAL INCOME"
            };
            x = Core.Accounts.AggregateAllTimeIncomeTotal(out int B1, out int B2);
            myline.Second = Core.MoneyString(x);
            if (B2 > 0)
            {
                myline.Third = Core.MoneyString(B1);
                myline.Fourth = Core.MoneyString(B2);
            }
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "As percentage of total cost"
            };
            double pc = (double)Core.Accounts.AggregateAllTimeIncomeTotal(out int C1, out int C2) / Core.Accounts.AggregateAllTimeCostTotal(out int D1, out int D2);
            pc *= 100;
            myline.Second = pc.ToString("0.00", CultureInfo.CurrentCulture) + "%";
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "Average annual income"
            };
            x = Core.Accounts.AverageAnnualIncome(out int R1, out int R2);
            myline.Second = Core.MoneyString(x);
            if (R2 > 0)
            {
                myline.Third = Core.MoneyString(R1);
                myline.Fourth = Core.MoneyString(R2);
            }
            _summaryLines.Add(myline);

            myline = new Fourply
            {
                First = "As percentage of total cost"
            };
            pc = (double)Core.Accounts.AverageAnnualIncome(out int E1, out int E2) / Core.Accounts.AggregateAllTimeCostTotal(out int F1, out int F2);
            pc *= 100;
            myline.Second = pc.ToString("0.00", CultureInfo.CurrentCulture) + "%";
            _summaryLines.Add(myline);

        }

        private void RefreshIncomeItems()
        {
            _incomeLines.Clear();
            IncomeItem i;
            foreach (string k in Core.Accounts.IncomeItems.Keys)
            {
                i = Core.Accounts.IncomeItems[k];
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
                    };
                    if (i.Furnished) { l.Furnished = "Yes"; } else { l.Furnished = "No"; }
                    l.Received = i.DateReceived.ToShortDateString();
                    l.Sum = Core.MoneyString(i.AmountPence);
                    _incomeLines.Add(l);
                }
            }
        }

        private void RefreshExpenditureItems()
        {
            _expenditureLines.Clear();

            ExpenditureItem e;
            foreach (string k in Core.Accounts.ExpenditureItems.Keys)
            {
                e = Core.Accounts.ExpenditureItems[k];
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
                Ink = new IncomeItem(Spec: ii.Specification),
                Owner = this,
                ParamNewItem = false
            };
            if (wdw.ShowDialog() == true)
            {
                IncomeItem NewIi = new IncomeItem(Spec: wdw.Ink.Specification);
                if (NewIi.DateReceived == ii.DateReceived) // can edit existing item
                {
                    Core.Accounts.IncomeItems[il.Key].AmountPence = NewIi.AmountPence;
                    Core.Accounts.IncomeItems[il.Key].CoversPeriodFromDate = NewIi.CoversPeriodFromDate;
                    Core.Accounts.IncomeItems[il.Key].CoversPeriodToDate = NewIi.CoversPeriodToDate;
                    Core.Accounts.IncomeItems[il.Key].Furnished = NewIi.Furnished;
                    Core.Accounts.IncomeItems[il.Key].Rubric = NewIi.Rubric;
                }
                else
                // must replace with new item as date had changed
                {
                    Core.Accounts.IncomeItems.Remove(il.Key);
                    Core.Accounts.IncomeItems.Add(key: Core.Accounts.UniqueIncomeKey(NewIi.DateReceived), value: NewIi);
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
                Z = new ExpenditureItem(Spec: ei.Specification),
                ParamNewItem = false,
                Owner = this
            };
            if (wdw.ShowDialog() == true)
            {
                ExpenditureItem NewEi = new ExpenditureItem(Spec: wdw.Z.Specification);
                if (NewEi.PayDate == ei.PayDate) //  can edit existing item
                {
                    Core.Accounts.ExpenditureItems[el.Key].AllocatedTaxYear = NewEi.AllocatedTaxYear;
                    Core.Accounts.ExpenditureItems[el.Key].AmountPence = NewEi.AmountPence;
                    Core.Accounts.ExpenditureItems[el.Key].Category = NewEi.Category;
                    Core.Accounts.ExpenditureItems[el.Key].Rubric = NewEi.Rubric;
                }
                else
                // must replace with new item as date had changed
                {
                    Core.Accounts.ExpenditureItems.Remove(el.Key);
                    Core.Accounts.ExpenditureItems.Add(key: Core.Accounts.UniqueExpenditureKey(NewEi.PayDate), value: NewEi);
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
                IncomeItem NewIi = new IncomeItem(Spec: wdw.Ink.Specification);
                Core.Accounts.IncomeItems.Add(key: Core.Accounts.UniqueIncomeKey(NewIi.DateReceived), value: NewIi);
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
                ExpenditureItem NewEi = new ExpenditureItem(Spec: wdw.Z.Specification);
                Core.Accounts.ExpenditureItems.Add(key: Core.Accounts.UniqueExpenditureKey(NewEi.PayDate), value: NewEi);
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
            double scrX = System.Windows.SystemParameters.PrimaryScreenWidth;
            double scrY = System.Windows.SystemParameters.PrimaryScreenHeight;
            double winX = scrX * .98;
            double winY = scrY * .94;
            double Xm = (scrX - winX) / 2;
            double Ym = (scrY - winY) / 4;
            Width = winX;
            Height = winY;
            Left = Xm;
            Top = Ym;
        }
}
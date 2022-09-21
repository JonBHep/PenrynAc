using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PenrynAc;

public partial class PropertyDetailsWindow
{
    public PropertyDetailsWindow()
        {
            InitializeComponent();
            // Assign these events after InitializeComponent rather than in XAML so as to avoid events firing 
            // and triggering Null exceptions during InitializeComponent
            _originalSharingSchedule = _originalAddress = _address = _title = string.Empty;
            PurchaseDateTextBlock.Tag = _purchaseDate;
            TxtProperty.TextChanged += TxtProperty_TextChanged;
            TxtAddress.TextChanged += TxtAddress_TextChanged;
            TxtPurchaseCost.TextChanged += TxtPurchaseCost_TextChanged;
            BtnOk.Click += BtnOK_Click;
            BtnCancel.Click += BtnCancel_Click;
        }

        private string _title;
        private string _address;
        private DateTime _purchaseDate;
        private int _purchaseCost;
        private readonly SharingSchedule _shares = new SharingSchedule();

        private string _originalAddress;
        private DateTime _originalPurchaseDate;
        private string _originalSharingSchedule;
        private int _originalPurchaseCost;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextblockErrorTitle.Visibility = Visibility.Hidden;

            TxtAddress.Text = _originalAddress = _address = PropertyAccounts.Instance.PropertyAddress;

            _shares.Specification = PropertyAccounts.Instance.LandlordShares.Specification;
            LblSharing.Text = $"{_shares.NumberOfPhases} phase(s)";
            _originalSharingSchedule = _shares.Specification;

            _originalPurchaseCost = _purchaseCost = PropertyAccounts.Instance.PropertyPurchaseCost;
            TxtPurchaseCost.Text = ((decimal)_purchaseCost / 100).ToString(CultureInfo.CurrentCulture);

            _originalPurchaseDate = _purchaseDate = PropertyAccounts.Instance.PropertyPurchaseDate;
            PurchaseDateTextBlock.Text = _purchaseDate.ToShortDateString();

            BtnOk.IsEnabled = false;

        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Enablement()
        {
            BtnOk.IsEnabled = !(((_originalAddress == TxtAddress.Text) || string.IsNullOrWhiteSpace(TxtAddress.Text))
                && (_originalPurchaseCost == _purchaseCost)
                && (_originalPurchaseDate ==(DateTime)PurchaseDateTextBlock.Tag) && (_originalSharingSchedule == _shares.Specification));
        }

        private void TxtAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            _address = TxtAddress.Text;
            Enablement();
        }

        private void TxtProperty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Core.IsValidFileName(TxtProperty.Text))
            {
                TextblockErrorTitle.Visibility = Visibility.Visible;
                BtnOk.IsEnabled = false;
            }
            else
            {
                TextblockErrorTitle.Visibility = Visibility.Hidden;
                _title = TxtProperty.Text;
                Enablement();
            }
        }

       public string PropertyAddress => _address;

       public DateTime PropertyPurchaseDate => _purchaseDate;

       public int PropertyPurchaseCost => _purchaseCost;

       private void TxtPurchaseCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            string qs = TxtPurchaseCost.Text.Trim();
            if (string.IsNullOrWhiteSpace(qs))
            {
                LblPurchaseCostInterpret.Text = "Amount is missing";
                LblPurchaseCostInterpret.Foreground = Brushes.Red;
                _purchaseCost = 0;
            }
            else
            {
                // identical sub in income dialogue
                if (decimal.TryParse(qs, out decimal sAmount))
                {
                    LblPurchaseCostInterpret.Text = sAmount.ToString("C", CultureInfo.CurrentCulture);
                    if (sAmount <= 0)
                    {
                        LblPurchaseCostInterpret.Foreground = Brushes.Red;
                        _purchaseCost = 0;
                    }
                    else
                    {
                        LblPurchaseCostInterpret.Foreground = Brushes.Blue;
                        _purchaseCost = (int)sAmount * 100;
                    }
                }
                else
                {
                    LblPurchaseCostInterpret.Text = "Amount is not a valid number";
                    LblPurchaseCostInterpret.Foreground = Brushes.Red;
                    _purchaseCost = 0;
                }
                Enablement();
            }

        }

        // private void DtpPurchaseDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        // {
        //     _PurchaseDate = DtpPurchaseDate.SelectedDate ?? DateTime.Today;
        //     Enablement();
        // }

        private void SharingButton_Click(object sender, RoutedEventArgs e)
        {
            PropertySharesWindow win = new PropertySharesWindow(_title) { Owner = this };
            win.SetSchedule(_shares.Specification);
            bool? q = win.ShowDialog();
            if ((q.HasValue) && (q.Value))
            {
                _shares.Specification = win.OutputScheduleSpecification;
                LblSharing.Text = $"{_shares.NumberOfPhases} phase(s)";
                Enablement();
            }
        }

        public string PropertySharingSpecification => _shares.Specification;

        private void PurchaseDateButton_OnClick(object sender, RoutedEventArgs e)
        {
            DateChooserWindow w = new DateChooserWindow() {Owner = this};
            if (w.ShowDialog() == true)
            {
                DateTime d = w.SelectedDate;
                PurchaseDateTextBlock.Tag = d;
                PurchaseDateTextBlock.Text = d.ToShortDateString();
                Enablement();
            }
        }
}
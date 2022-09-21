using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PenrynAc;

public partial class PropertyDetailsWindow : Window
{
    public PropertyDetailsWindow()
        {
            InitializeComponent();
            // Assign these events after InitializeComponent rather than in XAML so as to avoid events firing 
            // and triggering Null exceptions during InitializeComponent
            TxtProperty.TextChanged += TxtProperty_TextChanged;
            TxtAddress.TextChanged += TxtAddress_TextChanged;
            TxtPurchaseCost.TextChanged += TxtPurchaseCost_TextChanged;
            DtpPurchaseDate.DisplayDateStart = new DateTime(year: 2000, month: 1, day: 1);
            DtpPurchaseDate.DisplayDateEnd = DateTime.Today;
            DtpPurchaseDate.SelectedDateChanged += DtpPurchaseDate_SelectedDateChanged;
            BtnOk.Click += BtnOK_Click;
            BtnCancel.Click += BtnCancel_Click;
        }

        private string _Title;
        private string _Address;
        private DateTime _PurchaseDate;
        private int _PurchaseCost;
        private readonly SharingSchedule _shares = new SharingSchedule();

        private string originalAddress;
        private DateTime originalPurchaseDate;
        private string originalSharingSchedule;
        private int originalPurchaseCost;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextblockErrorTitle.Visibility = Visibility.Hidden;

            //txtProperty.Text = originalTitle = _Title = Core.Accounts.PropertyName;

            TxtAddress.Text = originalAddress = _Address = PropertyAccounts.Instance.PropertyAddress;

            _shares.Specification = PropertyAccounts.Instance.LandlordShares.Specification;
            LblSharing.Text = $"{_shares.NumberOfPhases} phase(s)";
            originalSharingSchedule = _shares.Specification;

            originalPurchaseCost = _PurchaseCost = PropertyAccounts.Instance.PropertyPurchaseCost;
            TxtPurchaseCost.Text = ((decimal)_PurchaseCost / 100).ToString(CultureInfo.CurrentCulture);

            originalPurchaseDate = _PurchaseDate = PropertyAccounts.Instance.PropertyPurchaseDate;
            DtpPurchaseDate.SelectedDate = _PurchaseDate;

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
            BtnOk.IsEnabled = !(((originalAddress == TxtAddress.Text) || string.IsNullOrWhiteSpace(TxtAddress.Text))
                && (originalPurchaseCost == _PurchaseCost)
                && (originalPurchaseDate == DtpPurchaseDate.SelectedDate) && (originalSharingSchedule == _shares.Specification));
        }

        private void TxtAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            _Address = TxtAddress.Text;
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
                _Title = TxtProperty.Text;
                Enablement();
            }
        }

        //public int PropertyShareSixths
        //{ get { return _ShareSixths; } }

        public string PropertyTitle
        { get { return _Title; } }

        public string PropertyAddress
        { get { return _Address; } }

        public DateTime PropertyPurchaseDate
        { get { return _PurchaseDate; } }

        public int PropertyPurchaseCost
        { get { return _PurchaseCost; } }

        private void TxtPurchaseCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            string qs = TxtPurchaseCost.Text.Trim();
            if (string.IsNullOrWhiteSpace(qs))
            {
                LblPurchaseCostInterpret.Text = "Amount is missing";
                LblPurchaseCostInterpret.Foreground = Brushes.Red;
                _PurchaseCost = 0;
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
                        _PurchaseCost = 0;
                    }
                    else
                    {
                        LblPurchaseCostInterpret.Foreground = Brushes.Blue;
                        _PurchaseCost = (int)sAmount * 100;
                    }
                }
                else
                {
                    LblPurchaseCostInterpret.Text = "Amount is not a valid number";
                    LblPurchaseCostInterpret.Foreground = Brushes.Red;
                    _PurchaseCost = 0;
                }
                Enablement();
            }

        }

        private void DtpPurchaseDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _PurchaseDate = (DateTime)DtpPurchaseDate.SelectedDate;
            Enablement();
        }

        private void SharingButton_Click(object sender, RoutedEventArgs e)
        {
            PropertySharesWindow win = new PropertySharesWindow(_Title) { Owner = this };
            win.SetSchedule(_shares.Specification);
            bool? Q = win.ShowDialog();
            if ((Q.HasValue) && (Q.Value))
            {
                _shares.Specification = win.OutputScheduleSpecification;
                LblSharing.Text = $"{_shares.NumberOfPhases} phase(s)";
                Enablement();
            }
        }

        public string PropertySharingSpecification { get { return _shares.Specification; } }
}
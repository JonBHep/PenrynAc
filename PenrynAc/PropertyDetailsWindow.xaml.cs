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
            txtProperty.TextChanged += TxtProperty_TextChanged;
            txtAddress.TextChanged += TxtAddress_TextChanged;
            txtPurchaseCost.TextChanged += TxtPurchaseCost_TextChanged;
            dtpPurchaseDate.DisplayDateStart = new DateTime(year: 2000, month: 1, day: 1);
            dtpPurchaseDate.DisplayDateEnd = DateTime.Today;
            dtpPurchaseDate.SelectedDateChanged += DtpPurchaseDate_SelectedDateChanged;
            btnOK.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
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
            textblockErrorTitle.Visibility = Visibility.Hidden;

            //txtProperty.Text = originalTitle = _Title = Core.Accounts.PropertyName;

            txtAddress.Text = originalAddress = _Address = Core.Accounts.PropertyAddress;

            _shares.Specification = Core.Accounts.LandlordShares.Specification;
            lblSharing.Text = $"{_shares.NumberOfPhases} phase(s)";
            originalSharingSchedule = _shares.Specification;

            originalPurchaseCost = _PurchaseCost = Core.Accounts.PropertyPurchaseCost;
            txtPurchaseCost.Text = ((decimal)_PurchaseCost / 100).ToString(CultureInfo.CurrentCulture);

            originalPurchaseDate = _PurchaseDate = Core.Accounts.PropertyPurchaseDate;
            dtpPurchaseDate.SelectedDate = _PurchaseDate;

            btnOK.IsEnabled = false;

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
            btnOK.IsEnabled = !(((originalAddress == txtAddress.Text) || string.IsNullOrWhiteSpace(txtAddress.Text))
                && (originalPurchaseCost == _PurchaseCost)
                && (originalPurchaseDate == dtpPurchaseDate.SelectedDate) && (originalSharingSchedule == _shares.Specification));
        }

        private void TxtAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            _Address = txtAddress.Text;
            Enablement();
        }

        private void TxtProperty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Core.IsValidFileName(txtProperty.Text))
            {
                textblockErrorTitle.Visibility = Visibility.Visible;
                btnOK.IsEnabled = false;
            }
            else
            {
                textblockErrorTitle.Visibility = Visibility.Hidden;
                _Title = txtProperty.Text;
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
            string qs = txtPurchaseCost.Text.Trim();
            if (string.IsNullOrWhiteSpace(qs))
            {
                lblPurchaseCostInterpret.Text = "Amount is missing";
                lblPurchaseCostInterpret.Foreground = Brushes.Red;
                _PurchaseCost = 0;
            }
            else
            {
                // identical sub in income dialogue
                if (decimal.TryParse(qs, out decimal sAmount))
                {
                    lblPurchaseCostInterpret.Text = sAmount.ToString("C", CultureInfo.CurrentCulture);
                    if (sAmount <= 0)
                    {
                        lblPurchaseCostInterpret.Foreground = Brushes.Red;
                        _PurchaseCost = 0;
                    }
                    else
                    {
                        lblPurchaseCostInterpret.Foreground = Brushes.Blue;
                        _PurchaseCost = (int)sAmount * 100;
                    }
                }
                else
                {
                    lblPurchaseCostInterpret.Text = "Amount is not a valid number";
                    lblPurchaseCostInterpret.Foreground = Brushes.Red;
                    _PurchaseCost = 0;
                }
                Enablement();
            }

        }

        private void DtpPurchaseDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _PurchaseDate = (DateTime)dtpPurchaseDate.SelectedDate;
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
                lblSharing.Text = $"{_shares.NumberOfPhases} phase(s)";
                Enablement();
            }
        }

        public string PropertySharingSpecification { get { return _shares.Specification; } }
}
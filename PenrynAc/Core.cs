using System;
using System.Globalization;
using System.Windows.Media;

namespace PenrynAc;

public static class Core
{
    // Bitmap.ConvertToBitmapImage extension method
        // public static System.Windows.Media.Imaging.BitmapImage ConvertToBitmapImage(this System.Drawing.Bitmap bmp)
        // // this is useful e.g. for retrieving a bitmap from Project Properties.Resources and displaying it in
        // // an image control which requires a BitMapImage as its ImageSource
        // {
        //     if (bmp is null) { throw new ArgumentNullException(paramName: nameof( bmp)); }
        //     System.IO.MemoryStream memory = new System.IO.MemoryStream();
        //     bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
        //     memory.Position = 0;
        //     System.Windows.Media.Imaging.BitmapImage bmpImage = new System.Windows.Media.Imaging.BitmapImage();
        //     bmpImage.BeginInit();
        //     bmpImage.StreamSource = memory;
        //     bmpImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
        //     bmpImage.EndInit();
        //     return bmpImage;
        // }

        public static System.Windows.Media.Color ToWPFColor(this System.Drawing.Color originalColor)
        // Extension method for color
        {
            return System.Windows.Media.Color.FromArgb(originalColor.A, originalColor.R, originalColor.G, originalColor.B);
        }

        public static string MoneyString(int amt)
        {
            decimal dml = ((decimal)amt) / 100;
            return dml.ToString("C", CultureInfo.CurrentCulture);
        }

        public const int TopExpenditureCategory = 5;

        public enum ExpenditureCategoryConstant
        {
            Unknown = 0,
            AllowableAgentFees = 1,
            AllowableRatesInsurance = 2,
            AllowableRepairsMaintenanceRenewals = 3,
            NonAllowableSettingUpCosts = 4,
            NonAllowableOther = 5
        }

        public static string ExpenditureCategoryCaption(ExpenditureCategoryConstant ec)
        {
            switch (ec)
            {
                case ExpenditureCategoryConstant.AllowableAgentFees:
                    return "Agent fees";
                case ExpenditureCategoryConstant.AllowableRatesInsurance:
                    return "Council tax, utilities, insurance";
                case ExpenditureCategoryConstant.AllowableRepairsMaintenanceRenewals:
                    return "Repairs, maintenance, renewals";
                case ExpenditureCategoryConstant.NonAllowableOther:
                    return "Other costs not to be claimed against tax";
                case ExpenditureCategoryConstant.NonAllowableSettingUpCosts:
                    return "Setting-up costs not allowable against tax";
                default:
                    return "Unknown";
            }
        }

        public static bool ExpenditureReducingTaxableProfit(ExpenditureCategoryConstant ec)
        {
            switch (ec)
            {
                case ExpenditureCategoryConstant.AllowableAgentFees:
                    return true;
                case ExpenditureCategoryConstant.AllowableRatesInsurance:
                    return true;
                case ExpenditureCategoryConstant.AllowableRepairsMaintenanceRenewals:
                    return true;
                case ExpenditureCategoryConstant.NonAllowableOther:
                    return false;
                case ExpenditureCategoryConstant.NonAllowableSettingUpCosts:
                    return false;
                default:
                    return false;
            }
        }

        public static string AppRevDateString()
        {
            string exam = System.Reflection.Assembly.GetExecutingAssembly().Location;

            System.IO.FileInfo fi = new System.IO.FileInfo(exam);
            System.DateTime revDate = fi.LastWriteTime;
            System.TimeSpan ts = System.DateTime.Today - revDate;
            int ds = (int)ts.TotalDays;
            string rv = "Application revised " + revDate.ToShortDateString();
            rv += " (" + ds.ToString(CultureInfo.CurrentCulture);
            if (ds == 1) { rv += " day ago)"; } else { rv += " days ago)"; }
            return rv;
        }

        // Tax Year is identified by the first of its two years
        // e.g. Tax Year 2013-14 is referred to as 2013

        public static DateTime TaxYearStartDate(int firstYear)
        { return new DateTime(year: firstYear, month: 4, day: 6); }

        public static DateTime TaxYearEndDate(int firstYear)
        { return new DateTime(year: firstYear + 1, month: 4, day: 5); }

        public static int TaxYearFromDate(DateTime d)
        {
            int rv = d.Year;
            if (d.Month < 4) rv--;
            if ((d.Month == 4) && (d.Day < 6)) rv--;
            return rv;
        }

        public static string DateString(DateTime d)
        { return d.ToString("yyyyMMdd", CultureInfo.CurrentCulture); }

        public static DateTime DateFromString(string ds)
        {
            if (ds is null) { throw new ArgumentNullException(nameof(ds)); }
            int y = int.Parse(ds.Substring(0, 4), CultureInfo.CurrentCulture);
            int m = int.Parse(ds.Substring(4, 2), CultureInfo.CurrentCulture);
            int d = int.Parse(ds.Substring(6, 2), CultureInfo.CurrentCulture);
            return new DateTime(year: y, month: m, day: d);
        }

        public static bool IsValidFileName(string name)
        {
            // Determines whether the name is Empty/Null
            if (string.IsNullOrWhiteSpace(name)) return false;
            // Determines whether there are bad characters in the name. 
            foreach (char badChar in System.IO.Path.GetInvalidFileNameChars())
            {
                if (name.Contains(badChar)) return false;
            }
            // The name passes basic validation.
            return true;
        }

        public static string LastExpItemRubric { get; set; }
        public static string LastIncItemRubric { get; set; }
        public static int LastIncItemAmount { get; set; }
        public static int LastExpItemAmount { get; set; }
        public static Typeface FixedFont { get; set; } = new System.Windows.Media.Typeface(fontFamily: new System.Windows.Media.FontFamily("Consolas, Lucida Console, Courier New"), style: System.Windows.FontStyles.Normal, weight: System.Windows.FontWeights.Normal, stretch: System.Windows.FontStretches.Normal);
        public static char JSeparator { get; set; } = char.Parse("^");
        public static Typeface FixedFontBold { get; set; } = new System.Windows.Media.Typeface(fontFamily: new System.Windows.Media.FontFamily("Consolas, Lucida Console, Courier New"), style: System.Windows.FontStyles.Normal, weight: System.Windows.FontWeights.Bold, stretch: System.Windows.FontStretches.Normal);
        public static PropertyAccounts Accounts { get; set; }
    }

public struct JbhListItem : IEquatable<JbhListItem>
{
    readonly int _intValue;
    readonly string _rubric;

    public JbhListItem(int ItemIndex, string ItemRubric)
    {
        _intValue = ItemIndex;
        _rubric = ItemRubric;
    }

    public override string ToString()
    {
        return _rubric;
    }

    public int ItemIndex
    {
        get { return _intValue; }
    }

    public override bool Equals(Object obj)
    {
        bool eq = true;
        if (obj is JbhListItem itm)
        {
            if (!ItemIndex.Equals(itm.ItemIndex))
            {
                eq = false;
            }

            if (!this.ToString().Equals(itm.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                eq = false;
            }

            return eq;
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = (int) 2166136261;
            // Suitable nullity checks etc, of course :)
            hash = (hash * 16777619) ^ _intValue.GetHashCode();
            hash = (hash * 16777619) ^ _rubric.GetHashCode();
            return hash;
        }
    }

    public bool Equals(JbhListItem other)
    {
        bool eq = true;
        if (!ItemIndex.Equals(other.ItemIndex))
        {
            eq = false;
        }

        if (!this.ToString().Equals(other.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            eq = false;
        }

        return eq;
    }

    public static bool operator ==(JbhListItem left, JbhListItem right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JbhListItem left, JbhListItem right)
    {
        return !(left == right);
    }
}


using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace PenrynAc;

public static class Core
{

    // Uri.GetBitmapImage
    public static BitmapImage GetBitmapImage(this Uri imageAbsolutePath
        , BitmapCacheOption bitmapCacheOption = BitmapCacheOption.Default)
    {
        BitmapImage image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = bitmapCacheOption;
        image.UriSource = imageAbsolutePath;
        image.EndInit();

        return image;
    }

    public static string MoneyString(int amt)
    {
        decimal dml = ((decimal) amt) / 100;
        return dml.ToString("C", CultureInfo.CurrentCulture);
    }

    public const int TopExpenditureCategory = 5;

    public enum ExpenditureCategoryConstant
    {
        Unknown = 0
        , AllowableAgentFees = 1
        , AllowableRatesInsurance = 2
        , AllowableRepairsMaintenanceRenewals = 3
        , NonAllowableSettingUpCosts = 4
        , NonAllowableOther = 5
    }

    public static string ExpenditureCategoryCaption(ExpenditureCategoryConstant ec)
    {
        return ec switch
        {
            ExpenditureCategoryConstant.AllowableAgentFees => "Agent fees"
            , ExpenditureCategoryConstant.AllowableRatesInsurance => "Council tax, utilities, insurance"
            , ExpenditureCategoryConstant.AllowableRepairsMaintenanceRenewals => "Repairs, maintenance, renewals"
            , ExpenditureCategoryConstant.NonAllowableOther => "Other costs not to be claimed against tax"
            , ExpenditureCategoryConstant.NonAllowableSettingUpCosts => "Setting-up costs not allowable against tax"
            , _ => "Unknown"
        };
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
        var exam = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var fi = new System.IO.FileInfo(exam);
        var revDate = fi.LastWriteTime;
        var ts = DateTime.Today - revDate;
        var ds = (int) ts.TotalDays;
        var rv = "Application revised " + revDate.ToShortDateString();
        rv += " (" + ds.ToString(CultureInfo.CurrentCulture);
        if (ds == 1)
        {
            rv += " day ago)";
        }
        else
        {
            rv += " days ago)";
        }

        return rv;
    }

    // Tax Year is identified by the first of its two years
    // e.g. Tax Year 2013-14 is referred to as 2013

    public static DateTime TaxYearStartDate(int firstYear)
    {
        return new DateTime(year: firstYear, month: 4, day: 6);
    }

    public static DateTime TaxYearEndDate(int firstYear)
    {
        return new DateTime(year: firstYear + 1, month: 4, day: 5);
    }

    public static int TaxYearFromDate(DateTime d)
    {
        int rv = d.Year;
        if (d.Month < 4) rv--;
        if ((d.Month == 4) && (d.Day < 6)) rv--;
        return rv;
    }

    public static string DateString(DateTime d)
    {
        return d.ToString("yyyyMMdd", CultureInfo.CurrentCulture);
    }

    public static DateTime DateFromString(string ds)
    {
        if (ds is null)
        {
            throw new ArgumentNullException(nameof(ds));
        }

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

    public static string LastExpItemRubric { get; set; } = string.Empty;
    public static string LastIncItemRubric { get; set; } = string.Empty;
    public static int LastIncItemAmount { get; set; }
    public static int LastExpItemAmount { get; set; }

    public static char JSeparator { get; set; } = char.Parse("^");

   // public static PropertyAccounts Accounts { get; set; } = new ();

}





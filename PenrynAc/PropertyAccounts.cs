using System;
using System.Collections.Generic;

namespace PenrynAc;

public class PropertyAccounts
{
    public string PropertyAddress { get; set; }

    public int
        PropertyPurchaseCost
    {
        get;
        set;
    } // TODO make purchase cost into an expenditure category to allow for more than one cost i.e. purchase of my share and my subsequent purchase of R's share in 2 stages (Nov 18 then March 2019)

    public DateTime PropertyPurchaseDate { get; set; }
    public SharingSchedule LandlordShares { get; set; }

    // NOTE The key for exp and inc items will force sorting of items by date by being YYYYMMDDNNN where NNN ensures uniqueness
    // allows 999 each of inc and exp items per day. Changing the date of an item would require changing the key, and therefore
    // creating a new item to replace the old

    public PropertyAccounts(string propAddress, string SharingSpec, DateTime propPurchaseDate, int propPurchaseCost)
    {
        ExpenditureItems = new SortedDictionary<string, ExpenditureItem>();
        IncomeItems = new SortedDictionary<string, IncomeItem>();
        //PropertyName = propName;
        PropertyAddress = propAddress;
        PropertyPurchaseCost = propPurchaseCost;
        PropertyPurchaseDate = propPurchaseDate;
        LandlordShares = new SharingSchedule() {Specification = SharingSpec};
    }

    public PropertyAccounts()
    {
        ExpenditureItems = new SortedDictionary<string, ExpenditureItem>();
        IncomeItems = new SortedDictionary<string, IncomeItem>();
        LandlordShares = new SharingSchedule();
        LoadData();
    }

    public void LoadData()
    {
        string dataLine;
        string lineId;
        string lineContent;
        string NewKey;
        string filePath = Jbh.AppManager.DataPath;
        filePath = System.IO.Path.Combine(filePath, "PenrynAccounts.txt");
        using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
        {
            while (!sr.EndOfStream)
            {
                dataLine = sr.ReadLine();
                int w = dataLine.IndexOf(":", StringComparison.OrdinalIgnoreCase);
                lineId = dataLine.Substring(0, w);
                lineContent = dataLine.Substring(w + 1);
                switch (lineId)
                {
                    //case "Property": { PropertyName = lineContent; break; }
                    case "Address":
                    {
                        PropertyAddress = lineContent;
                        break;
                    }
                    case "LandlordShares":
                    {
                        LandlordShares = new SharingSchedule(lineContent);
                        break;
                    }
                    case "PurchaseCost":
                    {
                        PropertyPurchaseCost = int.Parse(lineContent, System.Globalization.CultureInfo.CurrentCulture);
                        break;
                    }
                    case "PurchaseDate":
                    {
                        PropertyPurchaseDate = Core.DateFromString(lineContent);
                        break;
                    }
                    case "Exp":
                    {
                        ExpenditureItem ei = new ExpenditureItem(Spec: lineContent);
                        NewKey = UniqueExpenditureKey(ei.PayDate);
                        ExpenditureItems.Add(key: NewKey, value: ei);
                        break;
                    }
                    case "Inc":
                    {
                        IncomeItem ii = new IncomeItem(Spec: lineContent);
                        NewKey = UniqueIncomeKey(ii.DateReceived);
                        IncomeItems.Add(key: NewKey, value: ii);
                        break;
                    }
                    case "DefaultIncItem":
                    {
                        int j = lineContent.IndexOf("@", StringComparison.OrdinalIgnoreCase);
                        string much = lineContent.Substring(0, j);
                        string what = lineContent.Substring(j + 1);
                        Core.LastIncItemAmount = int.Parse(much, System.Globalization.CultureInfo.CurrentCulture);
                        Core.LastIncItemRubric = what;
                        break;
                    }
                    case "DefaultExpItem":
                    {
                        int j = lineContent.IndexOf("@", StringComparison.OrdinalIgnoreCase);
                        string much = lineContent.Substring(0, j);
                        string what = lineContent.Substring(j + 1);
                        Core.LastExpItemAmount = int.Parse(much, System.Globalization.CultureInfo.CurrentCulture);
                        Core.LastExpItemRubric = what;
                        break;
                    }
                    default:
                    {
                        System.Windows.MessageBox.Show("Erroneous line Id in data file: " + lineId, "Lettings"
                            , System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        break;
                    }
                }
            }
        }
    }

    public void SaveData()
    {
        string filePath = Jbh.AppManager.DataPath;
        filePath = System.IO.Path.Combine(filePath, "PenrynAccounts.txt");
        Jbh.AppManager.CreateBackupDataFile(filePath);
        Jbh.AppManager.PurgeOldBackups("txt", 30, 20);

        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path: filePath))
        {
            sw.WriteLine("Address:" + PropertyAddress);
            sw.WriteLine("LandlordShares:" + LandlordShares.Specification);
            sw.WriteLine("PurchaseDate:" + Core.DateString(PropertyPurchaseDate));
            sw.WriteLine("PurchaseCost:" +
                         PropertyPurchaseCost.ToString(System.Globalization.CultureInfo.CurrentCulture));
            foreach (ExpenditureItem ei in ExpenditureItems.Values)
            {
                sw.WriteLine("Exp:" + ei.Specification);
            }

            foreach (IncomeItem ii in IncomeItems.Values)
            {
                sw.WriteLine("Inc:" + ii.Specification);
            }

            sw.WriteLine($"DefaultIncItem:{Core.LastIncItemAmount}@{Core.LastIncItemRubric}");
            sw.WriteLine($"DefaultExpItem:{Core.LastExpItemAmount}@{Core.LastExpItemRubric}");
        }
    }

    public string UniqueExpenditureKey(DateTime datePoint)
    {
        string putativeKey;
        string keyRoot = Core.DateString(datePoint);
        int keyIndex = -1;
        do
        {
            keyIndex++;
            putativeKey = keyRoot + keyIndex.ToString("000", System.Globalization.CultureInfo.CurrentCulture);
        } while (ExpenditureItems.ContainsKey(putativeKey));

        return putativeKey;
    }

    public string UniqueIncomeKey(DateTime datePoint)
    {
        string putativeKey;
        string keyRoot = Core.DateString(datePoint);
        int keyIndex = -1;
        do
        {
            keyIndex++;
            putativeKey = keyRoot + keyIndex.ToString("000", System.Globalization.CultureInfo.CurrentCulture);
        } while (IncomeItems.ContainsKey(putativeKey));

        return putativeKey;
    }

    public int AggregateCostSettingUp
    {
        get
        {
            bool includeItem;
            int running = 0;
            foreach (ExpenditureItem ei in ExpenditureItems.Values)
            {
                includeItem = false;
                if (ei.Category == Core.ExpenditureCategoryConstant.NonAllowableSettingUpCosts) includeItem = true;
                if (includeItem) running += ei.AmountPence;
            }

            return running;
        }
    }

    public int AggregateCostSubsequent(out int FirstLandord, out int SecondLandlord)
    {
        bool includeItem;
        int running = 0;
        FirstLandord = 0;
        SecondLandlord = 0;
        foreach (ExpenditureItem ei in ExpenditureItems.Values)
        {
            includeItem = true;
            if (ei.Category == Core.ExpenditureCategoryConstant.NonAllowableSettingUpCosts) includeItem = false;
            if (includeItem)
            {
                running += ei.AmountPence;
                FirstLandord += LandlordShares.FirstShare(ei.AmountPence, ei.PayDate);
                SecondLandlord += LandlordShares.SecondShare(ei.AmountPence, ei.PayDate);
            }
        }

        return running;
    }

    public int AggregateAllTimeCostTotal(out int FirstLandord, out int SecondLandlord)
    {
        int running = PropertyPurchaseCost;
        FirstLandord = LandlordShares.FirstShare(PropertyPurchaseCost, PropertyPurchaseDate);
        SecondLandlord = LandlordShares.SecondShare(PropertyPurchaseCost, PropertyPurchaseDate);
        foreach (ExpenditureItem ei in ExpenditureItems.Values)
        {
            running += ei.AmountPence;
            FirstLandord += LandlordShares.FirstShare(ei.AmountPence, ei.PayDate);
            SecondLandlord += LandlordShares.SecondShare(ei.AmountPence, ei.PayDate);
        }

        return running;
    }

    public int AggregateAllTimeIncomeTotal(out int FirstLandord, out int SecondLandlord)
    {
        int running = 0;
        FirstLandord = 0;
        SecondLandlord = 0;
        foreach (IncomeItem ii in IncomeItems.Values)
        {
            running += ii.AmountPence;
            FirstLandord += LandlordShares.FirstShare(ii.AmountPence, ii.DateReceived);
            SecondLandlord += LandlordShares.SecondShare(ii.AmountPence, ii.DateReceived);
        }

        return running;
    }

    public double YearsRun
    {
        get
        {
            TimeSpan ts = (DateTime.Today - PropertyPurchaseDate);
            return ts.TotalDays / 365.25;
        }
    }

    public SortedDictionary<string, ExpenditureItem> ExpenditureItems { get; }
    public SortedDictionary<string, IncomeItem> IncomeItems { get; }

    public int AverageAnnualIncome(out int P1, out int P2)
    {
        double i;
        double z1;
        double z2;
        if (YearsRun < 1)
        {
            i = 0;
            z1 = 0;
            z2 = 0;
        }
        else
        {
            i = AggregateAllTimeIncomeTotal(out int Q1, out int Q2) / YearsRun;
            z1 = Q1 / YearsRun;
            z2 = Q2 / YearsRun;
        }

        P1 = (int) Math.Round(z1);
        P2 = (int) Math.Round(z2);
        return (int) i;
    }
}

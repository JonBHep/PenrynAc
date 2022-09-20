using System;
using System.Collections.Generic;

namespace PenrynAc;

public class PropertyAccounts
{
    public string PropertyAddress { get; set; }

    public int PropertyPurchaseCost
    {
        get;
        set;
    } // TODO make purchase cost into an expenditure category to allow for more than one cost i.e. purchase of my share and my
      // subsequent purchase of R's share in 2 stages (Nov 18 then March 2019)

    public DateTime PropertyPurchaseDate { get; set; }
    public SharingSchedule LandlordShares { get; private set; }

    // NOTE The key for exp and inc items will force sorting of items by date by being YYYYMMDDNNN where NNN ensures uniqueness
    // allows 999 each of inc and exp items per day. Changing the date of an item would require changing the key, and therefore
    // creating a new item to replace the old

    // public PropertyAccounts(string propAddress, string SharingSpec, DateTime propPurchaseDate, int propPurchaseCost)
    // {
    //     ExpenditureItems = new SortedDictionary<string, ExpenditureItem>();
    //     IncomeItems = new SortedDictionary<string, IncomeItem>();
    //     PropertyAddress = propAddress;
    //     PropertyPurchaseCost = propPurchaseCost;
    //     PropertyPurchaseDate = propPurchaseDate;
    //     LandlordShares = new SharingSchedule() {Specification = SharingSpec};
    // }

    public PropertyAccounts()
    {
        ExpenditureItems = new SortedDictionary<string, ExpenditureItem>();
        IncomeItems = new SortedDictionary<string, IncomeItem>();
        LandlordShares = new SharingSchedule();
        PropertyAddress=string.Empty;
        LoadData();
    }

    private void LoadData()
    {
        var filePath = Jbh.AppManager.DataPath;
        filePath = System.IO.Path.Combine(filePath, "PenrynAccounts.txt");
        using var sr = new System.IO.StreamReader(filePath);
        while (!sr.EndOfStream)
        {
            var dataLine = sr.ReadLine() ?? string.Empty;
            var w = dataLine.IndexOf(":", StringComparison.OrdinalIgnoreCase);
            var lineId = dataLine.Substring(0, w);
            var lineContent = dataLine.Substring(w + 1);
            string newKey;
            switch (lineId)
            {
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
                    var ei = new ExpenditureItem(Spec: lineContent);
                    newKey = UniqueExpenditureKey(ei.PayDate);
                    ExpenditureItems.Add(key: newKey, value: ei);
                    break;
                }
                case "Inc":
                {
                    var ii = new IncomeItem(Spec: lineContent);
                    newKey = UniqueIncomeKey(ii.DateReceived);
                    IncomeItems.Add(key: newKey, value: ii);
                    break;
                }
                case "DefaultIncItem":
                {
                    var j = lineContent.IndexOf("@", StringComparison.OrdinalIgnoreCase);
                    var much = lineContent.Substring(0, j);
                    var what = lineContent.Substring(j + 1);
                    Core.LastIncItemAmount = int.Parse(much, System.Globalization.CultureInfo.CurrentCulture);
                    Core.LastIncItemRubric = what;
                    break;
                }
                case "DefaultExpItem":
                {
                    var j = lineContent.IndexOf("@", StringComparison.OrdinalIgnoreCase);
                    var much = lineContent.Substring(0, j);
                    var what = lineContent.Substring(j + 1);
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

    public void SaveData()
    {
        var filePath = Jbh.AppManager.DataPath;
        filePath = System.IO.Path.Combine(filePath, "PenrynAccounts.txt");
        Jbh.AppManager.CreateBackupDataFile(filePath);
        Jbh.AppManager.PurgeOldBackups("txt", 30, 20);

        using var sw = new System.IO.StreamWriter(path: filePath);
        sw.WriteLine("Address:" + PropertyAddress);
        sw.WriteLine("LandlordShares:" + LandlordShares.Specification);
        sw.WriteLine("PurchaseDate:" + Core.DateString(PropertyPurchaseDate));
        sw.WriteLine("PurchaseCost:" +
                     PropertyPurchaseCost.ToString(System.Globalization.CultureInfo.CurrentCulture));
        foreach (var ei in ExpenditureItems.Values)
        {
            sw.WriteLine("Exp:" + ei.Specification);
        }

        foreach (var ii in IncomeItems.Values)
        {
            sw.WriteLine("Inc:" + ii.Specification);
        }

        sw.WriteLine($"DefaultIncItem:{Core.LastIncItemAmount}@{Core.LastIncItemRubric}");
        sw.WriteLine($"DefaultExpItem:{Core.LastExpItemAmount}@{Core.LastExpItemRubric}");
    }

    public string UniqueExpenditureKey(DateTime datePoint)
    {
        string putativeKey;
        var keyRoot = Core.DateString(datePoint);
        var keyIndex = -1;
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
        var keyRoot = Core.DateString(datePoint);
        var keyIndex = -1;
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
            var running = 0;
            foreach (var ei in ExpenditureItems.Values)
            {
                bool includeItem = ei.Category == Core.ExpenditureCategoryConstant.NonAllowableSettingUpCosts;
                if (includeItem) running += ei.AmountPence;
            }

            return running;
        }
    }

    public int AggregateCostSubsequent(out int firstLandlord, out int secondLandlord)
    {
        var running = 0;
        firstLandlord = 0;
        secondLandlord = 0;
        foreach (var ei in ExpenditureItems.Values)
        {
            bool includeItem = ei.Category != Core.ExpenditureCategoryConstant.NonAllowableSettingUpCosts;
            if (includeItem)
            {
                running += ei.AmountPence;
                firstLandlord += LandlordShares.FirstShare(ei.AmountPence, ei.PayDate);
                secondLandlord += LandlordShares.SecondShare(ei.AmountPence, ei.PayDate);
            }
        }

        return running;
    }

    public int AggregateAllTimeCostTotal(out int firstLandlord, out int secondLandlord)
    {
        var running = PropertyPurchaseCost;
        firstLandlord = LandlordShares.FirstShare(PropertyPurchaseCost, PropertyPurchaseDate);
        secondLandlord = LandlordShares.SecondShare(PropertyPurchaseCost, PropertyPurchaseDate);
        foreach (var ei in ExpenditureItems.Values)
        {
            running += ei.AmountPence;
            firstLandlord += LandlordShares.FirstShare(ei.AmountPence, ei.PayDate);
            secondLandlord += LandlordShares.SecondShare(ei.AmountPence, ei.PayDate);
        }

        return running;
    }

    public int AggregateAllTimeIncomeTotal(out int firstLandlord, out int secondLandlord)
    {
        var running = 0;
        firstLandlord = 0;
        secondLandlord = 0;
        foreach (var ii in IncomeItems.Values)
        {
            running += ii.AmountPence;
            firstLandlord += LandlordShares.FirstShare(ii.AmountPence, ii.DateReceived);
            secondLandlord += LandlordShares.SecondShare(ii.AmountPence, ii.DateReceived);
        }

        return running;
    }

    public double YearsRun
    {
        get
        {
            var ts = (DateTime.Today - PropertyPurchaseDate);
            return ts.TotalDays / 365.25;
        }
    }

    public SortedDictionary<string, ExpenditureItem> ExpenditureItems { get; }
    public SortedDictionary<string, IncomeItem> IncomeItems { get; }

    public int AverageAnnualIncome(out int p1, out int p2)
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
            i = AggregateAllTimeIncomeTotal(out var q1, out var q2) / YearsRun;
            z1 = q1 / YearsRun;
            z2 = q2 / YearsRun;
        }

        p1 = (int) Math.Round(z1);
        p2 = (int) Math.Round(z2);
        return (int) i;
    }
}

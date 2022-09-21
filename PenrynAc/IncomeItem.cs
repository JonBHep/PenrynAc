using System;
using System.Globalization;

namespace PenrynAc;

public class IncomeItem
{
    public string Rubric { get; set; }
    public int AmountPence { get; set; }
    public bool Furnished { get; set; }

    private DateTime _coversPeriodFromDate;
    private DateTime _coversPeriodToDate;

    public string Specification
    {
        get
        {
            string spcn = Core.DateString(DateReceived) + Core.JSeparator;
            spcn += Rubric + Core.JSeparator;
            spcn += AmountPence.ToString(CultureInfo.CurrentCulture) + Core.JSeparator;
            spcn += Core.DateString(_coversPeriodFromDate) + Core.JSeparator;
            spcn += Core.DateString(_coversPeriodToDate) + Core.JSeparator;
            spcn += Furnished.ToString();
            return spcn;
        }
    }

    public IncomeItem(string spec) // overloaded class contructor
    {
        if (spec is null)
        {
            throw new ArgumentNullException(paramName: spec);
        }

        string[] qi = spec.Split(Core.JSeparator);
        DateReceived = Core.DateFromString(qi[0]);
        Rubric = qi[1];
        AmountPence = int.Parse(qi[2], CultureInfo.CurrentCulture);
        _coversPeriodFromDate = Core.DateFromString(qi[3]);
        _coversPeriodToDate = Core.DateFromString(qi[4]);
        Furnished = bool.Parse(qi[5]);
    }

    public IncomeItem(DateTime whenReceived, string description, int penceValue, DateTime periodFrom, DateTime periodTo
        , bool furnishd) // overloaded class contructor
    {
        DateReceived = whenReceived;
        Rubric = description;
        AmountPence = penceValue;
        _coversPeriodFromDate = periodFrom;
        _coversPeriodToDate = periodTo;
        Furnished = furnishd;
    }

    public DateTime DateReceived
    {
        // this is read only as it is only intended to be settable by a constructor
        // Changing the date means chainging the unique key, so requires a new replacement item
        get;
        private set;
    }

    public void ExceptionallySetReceivedDate(DateTime dr)
    {
        DateReceived = dr;
    } // Although DateReceived is readonly (because needs to match key for genuine items) we need a way to force this edit for a temp editing object

    public DateTime CoversPeriodFromDate
    {
        get => _coversPeriodFromDate;
        set => _coversPeriodFromDate = new DateTime(year: value.Year, month: value.Month, day: value.Day);
        // not simply = value as we want to ensure time element is always midnight
    }

    public DateTime CoversPeriodToDate
    {
        get => _coversPeriodToDate;
        set => _coversPeriodToDate = new DateTime(year: value.Year, month: value.Month, day: value.Day);
        // not simply = value as we want to ensure time element is always midnight
    }

    public int DaysCovered
    {
        get
        {
            if (_coversPeriodFromDate == _coversPeriodToDate)
                return 0; //  don't return a period of 1 day for non-period items
            DateTime eDate = _coversPeriodToDate.AddDays(1);
            TimeSpan q = eDate - _coversPeriodFromDate;
            return (int) q.TotalDays;
        }
    }

    public bool HitsYear(int targetYear, AnnualSummaryWindow.YearType typeOfYear)
    {
        bool retVal = false;
        switch (typeOfYear)
        {
            case AnnualSummaryWindow.YearType.CalendarYearAccrual:
            {
                DateTime yrStart = new DateTime(year: targetYear, month: 1, day: 1);
                DateTime yrEnd = new DateTime(year: targetYear, month: 12, day: 31);
                retVal = (_coversPeriodFromDate < yrEnd) && (_coversPeriodToDate >= yrStart);
                break;
            }
            case AnnualSummaryWindow.YearType.CalendarYearCash:
            {
                DateTime yrStart = new DateTime(year: targetYear, month: 1, day: 1);
                DateTime yrEnd = new DateTime(year: targetYear, month: 12, day: 31);
                retVal = (DateReceived <= yrEnd) && (DateReceived >= yrStart);
                break;
            }
            case AnnualSummaryWindow.YearType.TaxYearAccrual:
            {
                DateTime yrStart = Core.TaxYearStartDate(firstYear: targetYear);
                DateTime yrEnd = Core.TaxYearEndDate(firstYear: targetYear);
                retVal = (_coversPeriodFromDate < yrEnd) && (_coversPeriodToDate >= yrStart);
                break;
            }
            case AnnualSummaryWindow.YearType.TaxYearCashBasis:
            {
                DateTime yrStart = Core.TaxYearStartDate(firstYear: targetYear);
                DateTime yrEnd = Core.TaxYearEndDate(firstYear: targetYear);
                retVal = (DateReceived <= yrEnd) && (DateReceived >= yrStart);
                break;
            }
        }

        return retVal;
    }

    private DateTime StartDateInTaxYear(int taxYear)
    {
        DateTime yrStart = Core.TaxYearStartDate(firstYear: taxYear);
        if (_coversPeriodFromDate < yrStart)
        {
            return yrStart;
        }
        else
        {
            return _coversPeriodFromDate;
        }
    }

    private DateTime StartDateInCalendarYear(int calYear)
    {
        DateTime yrStart = new DateTime(year: calYear, month: 1, day: 1);
        if (_coversPeriodFromDate < yrStart)
        {
            return yrStart;
        }
        else
        {
            return _coversPeriodFromDate;
        }
    }

    private DateTime EndDateInTaxYear(int taxYear)
    {
        DateTime yrEnd = Core.TaxYearEndDate(firstYear: taxYear);
        if (_coversPeriodToDate > yrEnd)
        {
            return yrEnd;
        }
        else
        {
            return _coversPeriodToDate;
        }
    }

    private DateTime EndDateInCalendarYear(int calYear)
    {
        DateTime yrEnd = new DateTime(year: calYear, month: 12, day: 31);
        if (_coversPeriodToDate > yrEnd)
        {
            return yrEnd;
        }
        else
        {
            return _coversPeriodToDate;
        }
    }

    public int DaysCoveredInTaxYear(int txYear)
    {
        if (HitsYear(txYear, AnnualSummaryWindow.YearType.TaxYearAccrual) == false) return 0;
        DateTime eDate = EndDateInTaxYear(taxYear: txYear).AddDays(1);
        DateTime sDate = StartDateInTaxYear(taxYear: txYear);
        TimeSpan q = (eDate - sDate);
        return (int) q.TotalDays;
    }

    public int DaysCoveredInCalendarYear(int calYear)
    {
        if (HitsYear(calYear, AnnualSummaryWindow.YearType.CalendarYearAccrual) == false) return 0;
        DateTime eDate = EndDateInCalendarYear(calYear: calYear).AddDays(1);
        DateTime sDate = StartDateInCalendarYear(calYear: calYear);
        TimeSpan q = (eDate - sDate);
        return (int) q.TotalDays;
    }

    public int AmountPenceInTaxYear(int tYear)
    {
        if (HitsYear(tYear, AnnualSummaryWindow.YearType.TaxYearAccrual) == false) return 0;
        if (DaysCovered < 1) return AmountPence;
        int daysInYear = DaysCoveredInTaxYear(txYear: tYear);
        double proportion = (double) daysInYear / DaysCovered;
        return Convert.ToInt32(AmountPence * proportion);
    }

    public int AmountPenceInCalendarYear(int cYear)
    {
        if (HitsYear(cYear, AnnualSummaryWindow.YearType.CalendarYearAccrual) == false) return 0;
        if (DaysCovered < 1) return AmountPence;
        int daysInYear = DaysCoveredInCalendarYear(calYear: cYear);
        double proportion = (double) daysInYear / DaysCovered;
        return Convert.ToInt32(AmountPence * proportion);
    }
}
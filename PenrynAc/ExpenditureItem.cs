using System;
using System.Globalization;

namespace PenrynAc;

public class ExpenditureItem
{
    public string Rubric { get; set; }
    public int AmountPence { get; set; }
    public Core.ExpenditureCategoryConstant Category { get; set; }
    public int AllocatedTaxYear { get; set; }

    private DateTime _payDate;

    public DateTime PayDate => _payDate;

    // ReadOnly as it is only intended to be settable by a constructor
    // Changing the date means changing the unique key, so requires a new replacement item
    public void ExceptionallySetPayDate(DateTime dp)
    {
        _payDate = dp;
    } // Although PayDate is readonly (because needs to match key for genuine items) we need a way to force this edit for a temp editing object

    public string Specification
    {
        get
        {
            string spcn = Core.DateString(PayDate) + Core.JSeparator;
            spcn += AllocatedTaxYear.ToString(CultureInfo.CurrentCulture) + Core.JSeparator;
            spcn += Rubric + Core.JSeparator;
            spcn += AmountPence.ToString(CultureInfo.CurrentCulture) + Core.JSeparator;
            spcn += ((int) Category).ToString(CultureInfo.CurrentCulture) + Core.JSeparator;
            return spcn;
        }
    }

    public ExpenditureItem(string spec) // overloaded class constructor
    {
        if (spec is null)
        {
            throw new ArgumentNullException(paramName: spec);
        }

        string[] qi = spec.Split(Core.JSeparator);
        _payDate = Core.DateFromString(qi[0]);
        AllocatedTaxYear = int.Parse(qi[1], CultureInfo.CurrentCulture);
        Rubric = qi[2];
        AmountPence = int.Parse(qi[3], CultureInfo.CurrentCulture);
        int y = int.Parse(qi[4], CultureInfo.CurrentCulture); // convert string to int
        Category = (Core.ExpenditureCategoryConstant) y; // then convert int to enumeration constant type
    }

    public ExpenditureItem(DateTime taxPt, int allocTaxYr, string description, int penceValue
        , Core.ExpenditureCategoryConstant categ) // overloaded class constructor
    {
        _payDate = new DateTime(year: taxPt.Year, month: taxPt.Month, day: taxPt.Day);
        // not simply = taxpt because we want to exclude any time element (and thus default to midnight)
        AllocatedTaxYear = allocTaxYr;
        Rubric = description;
        AmountPence = penceValue;
        Category = categ;
    }
    
    public string TaxYearString =>
        AllocatedTaxYear.ToString(CultureInfo.CurrentCulture) + " - " +
        (AllocatedTaxYear + 1).ToString(CultureInfo.CurrentCulture);
}
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PenrynAc;

public class SharingSchedule
{
    private readonly string _mainSeparator = "^";

    public List<Tuple<DateTime, double>> Schedule { get; } = new List<Tuple<DateTime, double>>();

    public SharingSchedule()
    {
        Schedule = new List<Tuple<DateTime, double>>
        {
            new Tuple<DateTime, double>(new DateTime(1950, 1, 1), 1)
        };
    }

    public SharingSchedule(string specif)
    {
        Specification = specif;
    }

    public string Specification
    {
        get
        {
            var answ = string.Empty;
            foreach (var tup in Schedule)
            {
                var sub = $"{tup.Item1.ToString("yyyyMMdd", CultureInfo.CurrentCulture)}{tup.Item2}";
                answ += _mainSeparator + sub;
            }

            if (answ.Length > 0)
            {
                answ = answ.Substring(1);
            } // chop off the initial separator character

            return answ;
        }
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName: value);
            }

            var seps = _mainSeparator.ToCharArray();
            var bits = value.Split(seps);
            Schedule.Clear();
            foreach (var s in bits)
            {
                var ystring = s.Substring(0, 4);
                var mstring = s.Substring(4, 2);
                var dstring = s.Substring(6, 2);
                var propstring = s.Substring(8);
                var yr = int.Parse(ystring, CultureInfo.CurrentCulture);
                var mh = int.Parse(mstring, CultureInfo.CurrentCulture);
                var dy = int.Parse(dstring, CultureInfo.CurrentCulture);
                var prop = double.Parse(propstring, CultureInfo.CurrentCulture);
                var dt = new DateTime(yr, mh, dy);
                var phase = new Tuple<DateTime, double>(dt, prop);
                Schedule.Add(phase);
            }
        }
    }

    private double FirstShareApplicableOnDate(DateTime when)
    {
        double retval = 1;
        Schedule.Sort();
        foreach (var tup in Schedule)
        {
            if (tup.Item1 <= when)
            {
                retval = tup.Item2;
            }
        }

        return retval;
    }

    public int FirstShare(int wholeAmount, DateTime payDate)
    {
        var prop = FirstShareApplicableOnDate(payDate);
        var retval = wholeAmount * prop;
        return (int) Math.Round(retval);
    }

    public int SecondShare(int wholeAmount, DateTime payDate)
    {
        var prop = FirstShareApplicableOnDate(payDate);
        prop = 1 - prop;
        var retval = wholeAmount * prop;
        return (int) Math.Round(retval);
    }

    public int NumberOfPhases => Schedule.Count;

    public bool HasChangeInPeriod(DateTime startDate, DateTime endDate)
    {
        var change = false;
        foreach (var tup in Schedule)
        {
            var q = tup.Item1;
            var intime = !(q < startDate);

            if (q > endDate)
            {
                intime = false;
            }

            if (intime)
            {
                change = true;
            }
        }

        return change;
    }

    public bool SharingInYearStarting(DateTime startDate)
    {
        // Each change in sharing was a decrease, so if there is sharing at any time during the year if and only if there is sharing on the first day of the period 
        return (FirstShareApplicableOnDate(startDate) < 1);
    }
}

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
            string answ = string.Empty;
            foreach (Tuple<DateTime, double> tup in Schedule)
            {
                string sub = $"{tup.Item1.ToString("yyyyMMdd", CultureInfo.CurrentCulture)}{tup.Item2}";
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

            char[] seps = _mainSeparator.ToCharArray();
            string[] bits = value.Split(seps);
            Schedule.Clear();
            foreach (string s in bits)
            {
                string ystring = s.Substring(0, 4);
                string mstring = s.Substring(4, 2);
                string dstring = s.Substring(6, 2);
                string propstring = s.Substring(8);
                int yr = int.Parse(ystring, CultureInfo.CurrentCulture);
                int mh = int.Parse(mstring, CultureInfo.CurrentCulture);
                int dy = int.Parse(dstring, CultureInfo.CurrentCulture);
                double prop = double.Parse(propstring, CultureInfo.CurrentCulture);
                DateTime dt = new DateTime(yr, mh, dy);
                Tuple<DateTime, double> phase = new Tuple<DateTime, double>(dt, prop);
                Schedule.Add(phase);
            }
        }
    }

    private double FirstShareApplicableOnDate(DateTime when)
    {
        double retval = 1;
        Schedule.Sort();
        foreach (Tuple<DateTime, double> tup in Schedule)
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
        double prop = FirstShareApplicableOnDate(payDate);
        double retval = wholeAmount * prop;
        return (int) Math.Round(retval);
    }

    public int SecondShare(int wholeAmount, DateTime payDate)
    {
        double prop = FirstShareApplicableOnDate(payDate);
        prop = 1 - prop;
        double retval = wholeAmount * prop;
        return (int) Math.Round(retval);
    }

    public int NumberOfPhases
    {
        get { return Schedule.Count; }
    }

    public bool HasChangeInPeriod(DateTime startDate, DateTime endDate)
    {
        bool change = false;
        foreach (Tuple<DateTime, double> tup in Schedule)
        {
            DateTime Q = tup.Item1;
            bool intime = true;
            if (Q < startDate)
            {
                intime = false;
            }

            if (Q > endDate)
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

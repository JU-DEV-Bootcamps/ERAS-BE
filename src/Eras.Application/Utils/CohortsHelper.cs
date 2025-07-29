using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eras.Application.Utils;
public class CohortsHelper
{
    public static string GetCohort(DateTime SelectedDate)
    {
        int year = SelectedDate.Year;
        int cohortNumber = SelectedDate.Month <= 6 ? 1 : 2;

        return $"Cohort {cohortNumber} ({year})";
    }

    public static List<string> GetCohortsFromDateRange(DateTime StartDate, DateTime EndDate)
    {
        var cohorts = new List<string>();
        if (StartDate > EndDate)
        {
            var temp = StartDate;
            StartDate = EndDate;
            EndDate = temp;
        }

        DateTime dateTime = new DateTime(StartDate.Year, StartDate.Month <= 6 ? 1 : 7, 1);

        while (dateTime <= EndDate)
        {
            string cohort = GetCohort(dateTime);
            if (!cohorts.Contains(cohort))
            {
                cohorts.Add(cohort);
            }
            dateTime = dateTime.AddMonths(6);
        }
        return cohorts;
    }

    public static string NormalizeCohort(string Cohort)
    {
        var regex = new Regex(@"Cohort\s([12])\s*\(\s*.*?(\d{4})\s*\)", RegexOptions.IgnoreCase);
        var match = regex.Match(Cohort);
        if (match.Success)
        {
            string cohortNumber = match.Groups[1].Value;
            string year = match.Groups[2].Value;
            return $"Cohort {cohortNumber} ({year})";
        }
        return string.Empty;
    }

    public static bool CohortInDateRange(string Cohort, DateTime StartDate, DateTime EndDate)
    {
        string normalizedCohort = NormalizeCohort(Cohort);
        var cohortsInRange = GetCohortsFromDateRange(StartDate, EndDate);
        return cohortsInRange.Contains(normalizedCohort);
    }
}

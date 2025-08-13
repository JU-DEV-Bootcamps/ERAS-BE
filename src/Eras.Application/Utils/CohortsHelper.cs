using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eras.Application.Utils;
public class CohortsHelper
{
    public static DateTime? ParseCohortDate(string Cohort)
    {
        var regex = new Regex(@"\(\s*([A-Za-zÁÉÍÓÚáéíóúñ]+)\s+(\d{4})\s*\)", RegexOptions.IgnoreCase);
        var match = regex.Match(Cohort);
        if (match.Success)
        {
            string monthName = match.Groups[1].Value;
            string year = match.Groups[2].Value;

            // Try parsing with Spanish culture first, then fallback to English
            string dateString = $"01 {monthName} {year}";
            string[] cultures = { "es-ES", "en-US" };

            foreach (var culture in cultures)
            {
                if (DateTime.TryParseExact(dateString, "dd MMMM yyyy", new CultureInfo(culture), DateTimeStyles.None, out DateTime parsedDate))
                {
                    return parsedDate;
                }
            }
        }
        return null;
    }

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
            (StartDate, EndDate) = (EndDate, StartDate);
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

    public static bool CohortInDateRange(string Cohort, DateTime StartDate, DateTime EndDate)
    {
        var cohortDate = ParseCohortDate(Cohort);
        if (cohortDate.HasValue)
        {
            return cohortDate.Value >= StartDate && cohortDate.Value <= EndDate;
        }
        return false;
    }
}

using System.Text.RegularExpressions;

namespace Eras.Application.Utils
{
    public static class SqlInjectionValidator
    {
        private static readonly Regex SqlInjectionPattern = new Regex(
            @"(\b(ALTER|CREATE|DELETE|DROP|EXEC(UTE){0,1}|INSERT( +INTO){0,1}|MERGE|SELECT|UPDATE|UNION( +ALL){0,1})\b)|" +
            @"(\b(SP_|XP_)\w+)|(--|#|\/\*|\*\/)|" +
            @"(\b(AND|OR)\b.*(=|LIKE|\bIN\b|\bBETWEEN\b))|" +
            @"([\'\""]\s*(\b(OR|AND)\b|[><!=]))|" +
            @"(\b(CHAR|ASCII|SUBSTRING|LEN|CONVERT|CAST|CONCAT)\s*\()|" +
            @"(\bUNION\b.*\bSELECT\b)|" +
            @"(\;\s*\b(ALTER|CREATE|DELETE|DROP|EXEC|INSERT|MERGE|SELECT|UPDATE|UNION)\b)|" +
            @"(\bWAITFOR\b)|" +
            @"(\bSHUTDOWN\b)|" +
            @"(\bBACKUP\b)|" +
            @"(\bRESTORE\b)|" +
            @"(\bCMD\b)|" +
            @"(\bSHELL\b)|" +
            @"(\bSYSTEM\b)|" +
            @"(\bOPENROWSET\b)|" +
            @"(\bOPENDATASOURCE\b)|" +
            @"(\bBULK\s+INSERT\b)|" +
            @"(\bxp_cmdshell\b)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public static bool IsValid(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            return !SqlInjectionPattern.IsMatch(input);
        }

        public static string Sanitize(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return SqlInjectionPattern.Replace(input, "").Trim();
        }
    }
}

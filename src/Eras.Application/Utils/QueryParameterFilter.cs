namespace Eras.Application.Utils;
public class QueryParameterFilter
{
    public static List<int> GetCohortIdsAsInts(string StrIds) => StrIds.Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(Id => int.TryParse(Id, out var parsed) ? parsed : 0)
            .Where(Id => Id != 0)
            .ToList();

}

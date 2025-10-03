using Employees.Interfaces;
using Employees.Models;

namespace Employees.Services;

public class EmployeeService : IEmployeeService
{
    public async Task<IEnumerable<EmployeePairResult>> GetEmployeePairs(IFormFile file)
    {
        var assignments = await LoadAssignmentsFromCsv(file);
        var pairsTotalOverlapByProject = new List<EmployeePairResult>();
        var groupedAssignments = assignments
            .GroupBy(a => a.ProjectId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var project in groupedAssignments)
        {
            var usersByProject = project.Value;

            // Store the best(biggest) overlap and pair per project
            var bestOverlap = 0L;
            (long, long) bestPair = (0, 0);

            for (var i = 0; i < usersByProject.Count; i++) 
            { 
                for (var j = i + 1; j < usersByProject.Count; j++)
                {
                    var rootUser = usersByProject[i];
                    var circularUser = usersByProject[j];

                    var overlap = OverlapInDays(rootUser.dateFrom, rootUser.DateTo, circularUser.dateFrom, circularUser.DateTo);

                    // Store only if current overlap is bigger then previous
                    if (overlap > 0 && overlap > bestOverlap)
                    {
                        bestOverlap = overlap;
                        bestPair = PrepareKey(rootUser.EmpId, circularUser.EmpId);
                    }
                }
            }

            if (bestOverlap > 0)
                pairsTotalOverlapByProject.Add(
                    new EmployeePairResult(
                        bestPair.Item1,
                        bestPair.Item2,
                        bestOverlap,
                        project.Key));
        }

        return pairsTotalOverlapByProject;
    }

    private static async Task<List<Assignment>> LoadAssignmentsFromCsv(IFormFile file)
    {
        var assignments = new List<Assignment>();

        using var reader = new StreamReader(file.OpenReadStream());
        string line = string.Empty;
        int lineNumber = 0;
        int expectedColumns = 4;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            //Headers
            if (lineNumber == 0) 
            {
                var expectedColumnNames = new[] { "EmpID", "ProjectID", "DateFrom", "DateTo" };
                var csvColumnNames = line.Split(',');

                for (int i = 0; i < csvColumnNames.Length; i++)
                {
                    var columnName = csvColumnNames[i].Trim();
                    if (!string.Equals(columnName, expectedColumnNames[i], StringComparison.OrdinalIgnoreCase))
                    {
                        throw new FormatException($"Invalid csv format. Expected column '{expectedColumnNames[i]}'," +
                            $"but found '{columnName}'");
                    }
                }

                lineNumber++;
                continue;
            }

            var columns = line.Split(",");
            if( columns.Length != expectedColumns)
            {
                throw new FormatException($"Invalid csv format. " +
                    $"The file should contain {expectedColumns} columns");
            }

            var empId = int.Parse(columns[0].Trim());
            var projectId = int.Parse(columns[1].Trim());
            var dateFrom = DateTime.Parse(columns[2].Trim());

            var dateToString = columns[3].Trim();
            var dateTo = string.IsNullOrEmpty(dateToString) || dateToString.Equals("NULL", StringComparison.OrdinalIgnoreCase)
                ? DateTime.Today
                : DateTime.Parse(dateToString);

            assignments.Add(new Assignment(empId, projectId, dateFrom, dateTo));
            lineNumber++;
        }

        return assignments;
    }

    private static (long, long) PrepareKey(long empId1, long empId2) => empId1 < empId2 ? (empId1, empId2) : (empId2, empId1);

    private static long OverlapInDays(
        DateTime rootUserFrom, 
        DateTime rootUserTo, 
        DateTime nextUserFrom, 
        DateTime nextUserTo)
    {
        var start = rootUserFrom > nextUserFrom ? rootUserFrom : nextUserFrom;
        var end = rootUserTo < nextUserTo ? rootUserTo : nextUserTo;

        TimeSpan interval = end - start;
        long days = interval >= TimeSpan.FromDays(1) ? interval.Days : 0;

        return days;
    }
}

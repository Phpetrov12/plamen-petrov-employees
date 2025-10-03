Employee Pair Analyzer

Small ASP.NET Core Web API that analyzes employee assignments and returns, for each project, the pair of employees who worked together the most (by total overlapping days).

Expected CSV format

The CSV must contain a header row with these exact column names (case-insensitive):
EmpID,ProjectID,DateFrom,DateTo

Date format: any parseable .NET date string (ISO yyyy-MM-dd recommended).

DateTo may be NULL or empty — treated as today.

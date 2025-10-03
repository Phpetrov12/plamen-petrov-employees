Employee Pair Analyzer

Small ASP.NET Core Web API that analyzes employee assignments and returns, for each project, the pair of employees who worked together the most (by total overlapping days).

Expected CSV format

The CSV must contain a header row with these exact column names (case-insensitive):
EmpID,ProjectID,DateFrom,DateTo

Date format: any parseable .NET date string (ISO yyyy-MM-dd recommended).

DateTo may be NULL or empty — treated as today.

Example assignments.csv:

EmpID,ProjectID,DateFrom,DateTo
143,12,2013-11-01,2014-01-05
218,10,2012-05-16,NULL
143,10,2009-01-01,2011-04-27
256,12,2013-12-01,2014-02-15
218,12,2013-11-15,2014-01-10

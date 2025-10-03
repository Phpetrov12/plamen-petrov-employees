namespace Employees.Models;

public record Assignment(long EmpId, long ProjectId, DateTime dateFrom, DateTime DateTo);
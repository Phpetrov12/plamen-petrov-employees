using Employees.Models;

namespace Employees.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeePairResult>> GetEmployeePairs(IFormFile file);
}

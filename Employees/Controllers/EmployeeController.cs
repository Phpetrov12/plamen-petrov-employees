using Employees.Interfaces;
using Employees.Models;
using Microsoft.AspNetCore.Mvc;

namespace Employees.Controllers
{
    [ApiController]
    [Route("employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService; 
        }

        [HttpPost("analyzer")]
        public async Task<ActionResult<List<EmployeePairResult>>> Get(IFormFile formFile)
        {
            try
            {
                var pairsResult = await _employeeService.GetEmployeePairs(formFile);
                if(!pairsResult.Any())
                    return NotFound();

                return Ok(pairsResult);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong, please try again later");
            }
        }
    }
}

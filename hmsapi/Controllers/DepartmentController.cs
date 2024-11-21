using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;
using hmsapi.Repositories;
using hmsapi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace cmsapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly SessionManager _sessionManager;
        private readonly IDbOperations _dbOperations;

        public DepartmentController(SessionManager sessionManager, IDbOperations dbOperations)
        {
            _sessionManager = sessionManager;
            _dbOperations = dbOperations;
        }

        [HttpPost("CreateDepartment")]
        public IActionResult CreateDepartment()
        {
            DaoResponse _drs = DepartmentRepo.CreateNewDepartment(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("UpdateDepartment")]
        public IActionResult UpdateDepartment()
        {
            DaoResponse _drs = DepartmentRepo.UpdateDepartment(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("DepartmentList")]
        public IActionResult DepartmentList()
        {
            DaoResponse _drs = DepartmentRepo.GetAllDepartments(_dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }
    }
}


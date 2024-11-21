
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
    public class DesignationController : ControllerBase
    {
        private readonly SessionManager _sessionManager;
        private readonly IDbOperations _dbOperations;

        public DesignationController(SessionManager sessionManager, IDbOperations dbOperations)
        {
            _sessionManager = sessionManager;
            _dbOperations = dbOperations;
        }

        [HttpPost("CreateDesignation")]
        public IActionResult CreateDesignation()
        {
            DaoResponse _drs = DesignationRepo.CreateNewDesignation(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("UpdateDesignation")]
        public IActionResult UpdateDesignation()
        {
            DaoResponse _drs = DesignationRepo.UpdateDesignation(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("DesignationList")]
        public IActionResult DesignationList()
        {
            DaoResponse _drs = DesignationRepo.GetAllDesignations(_dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }
    }
}


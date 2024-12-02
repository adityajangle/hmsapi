using System;
using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;
using hmsapi.Repositories;
using hmsapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace hmsapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PatientController:ControllerBase
	{
        private readonly SessionManager _sessionManager;
        private readonly IDbOperations _dbOperations;

        public PatientController(SessionManager sessionManager, IDbOperations dbOperations)
        {
            _sessionManager = sessionManager;
            _dbOperations = dbOperations;
        }

        [HttpPost("SearchPatient")]
        public IActionResult SearchPatient()
        {
            DaoResponse _drs = PatientRepo.SearchPatient(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

    }
}


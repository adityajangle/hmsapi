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
    public class AppointmentController:ControllerBase
	{
        private readonly SessionManager _sessionManager;
        private readonly IDbOperations _dbOperations;

        public AppointmentController(SessionManager sessionManager, IDbOperations dbOperations)
        {
            _sessionManager = sessionManager;
            _dbOperations = dbOperations;
        }

        [HttpPost("CreateAppointment")]
        public IActionResult CreateUser()
        {
            DaoResponse _drs = UserRepo.CreateNewUser(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("AppointmentList")]
        public IActionResult UpdateUser()
        {
            DaoResponse _drs = AppointmentRepo.GetAllAppointments( _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }
    }
}


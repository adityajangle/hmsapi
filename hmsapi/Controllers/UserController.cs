using System;
using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;
using hmsapi.Repositories;
using hmsapi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace hmsapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SessionManager _sessionManager;
        private readonly IDbOperations _dbOperations;

        public UserController(SessionManager sessionManager, IDbOperations dbOperations)
        {
            _sessionManager = sessionManager;
            _dbOperations = dbOperations;
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser()
        {
            DaoResponse _drs = UserRepo.CreateNewUser(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("UpdateUser")]
        public IActionResult UpdateUser()
        {
            DaoResponse _drs = UserRepo.UpdateUser(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("UserList")]
        public IActionResult UserList()
        {
            DaoResponse _drs = UserRepo.GetAllUsers(_dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("GetUserById")]
        public IActionResult GetUserById()
        {
            DaoResponse _drs = UserRepo.GetUserById(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }
    }
}


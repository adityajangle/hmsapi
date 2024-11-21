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
    public class RolesController : ControllerBase
    {
        private readonly SessionManager _sessionManager;
        private readonly IDbOperations _dbOperations;
        public RolesController(SessionManager sessionManager, IDbOperations dbOperations)
        {
            _sessionManager = sessionManager;
            _dbOperations = dbOperations;

        }

        [HttpPost("CreateRole")]
        public IActionResult CreateRole()
        {
            DaoResponse _drs = RoleRepo.CreateNewRole(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("UpdateRole")]
        public IActionResult UpdateRole()
        {
            DaoResponse _drs = RoleRepo.UpdateRole(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("GetRoleById")]
        public IActionResult GetRoleById()
        {
            DaoRequest request = _sessionManager.RequestData;
            DaoResponse _drs = RoleRepo.GetRoleById(request.Payload, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("RoleList")]
        public IActionResult RoleList()
        {
            DaoResponse _drs = RoleRepo.GetAllRoles(_dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("GetAllPermissions")]
        public IActionResult GetAllPermissions()
        {
            DaoResponse _drs = PermissionRepo.GetAllPermisions(_dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }
    }
}




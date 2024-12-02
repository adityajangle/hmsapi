using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;
using hmsapi.Repositories;
using hmsapi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;

namespace hmsapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConsentController : Controller
    {
        private readonly SessionManager _sessionManager;
        private readonly IDbOperations _dbOperations;

        public ConsentController(SessionManager sessionManager, IDbOperations dbOperations)
        {
            _sessionManager = sessionManager;
            _dbOperations = dbOperations;
        }
        [HttpPost("ConsentTypeList")]
        public IActionResult ConsenttypeList()
        {
            DaoResponse _drs = ConsentRepo.GetConsentList(_dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }
        [HttpPost("CreateConsent")]
        public IActionResult CreateConsent(DaoRequest daoRequest)
        {
            DaoResponse _drs = ConsentRepo.CreateNewConsent(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }
        
        [HttpPost("PatientConsentList")]
        public IActionResult PatientConsentList()
        {
            DaoResponse _drs = ConsentRepo.GetPatientConsentList(_sessionManager,_dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("PatientConsentListByAppointment")]
        public IActionResult PatientConsentListByAppointment()
        {
            DaoResponse _drs = ConsentRepo.GetPatientConsentListByAppointment(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }

        [HttpPost("GetConsentForm")]
        public IActionResult GetConsentForm()
        {
            DaoResponse _drs = ConsentRepo.GetConsentForm(_sessionManager, _dbOperations);
            return Ok(UtilService.ProcResponse(_drs, HttpContext, _sessionManager));
        }
    }
}
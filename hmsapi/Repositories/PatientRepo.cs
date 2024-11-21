using System;
using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;
using Newtonsoft.Json;

namespace hmsapi.Repositories
{
    public class PatientRepo
    {
        public static DaoResponse SearchPatient(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            Dictionary<string, object> prms = JsonConvert.DeserializeObject<Dictionary<string, object>>(request!.Payload!)!;




            return drs;
        }
    }
}


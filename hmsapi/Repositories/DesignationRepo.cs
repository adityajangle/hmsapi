
using Newtonsoft.Json;
using System.Data;
using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;

namespace hmsapi.Repositories
{
    public class DesignationRepo
    {
        public static DaoResponse GetAllDesignations(IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            List<Dictionary<string, object?>> desgList = new List<Dictionary<string, object?>>();
            mst_designation.GetAllDesignations(dbOperations).ForEach(x =>
            {
                Dictionary<string, object?> desgDict = new Dictionary<string, object?>();
                desgDict.Add("id", x.id);
                desgDict.Add("designation", x.name);
                desgDict.Add("users", "NA");
                desgDict.Add("created_on", x.created_date);
                desgDict.Add("status", x.is_active == DateOnly.FromDateTime(new DateTime(2100, 12, 31)));

                desgList.Add(desgDict);

            });

            drs.Status = true;
            drs.Message = "Successful";
            drs.Payload = JsonConvert.SerializeObject(desgList);
            return drs;
        }
        public static DaoResponse CreateNewDesignation(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            col_designation desg = JsonConvert.DeserializeObject<col_designation>(request!.Payload!)!;
            DataTable dtb = dbOperations.ExecuteTable("select count(*) from mst_designation where name=@name", new Dictionary<string, object>()
            {
                {"name", desg.name!}
            });
            DataTable dtb2 = dbOperations.ExecuteTable("select count(*) as ct from mst_designation where name=@name", new Dictionary<string, object>()
            {
                {"name", desg.name!}
            });
            if (Convert.ToInt32(dtb2.Rows[0]["ct"].ToString()) > 0)
            {
                _drs.Status = false;
                _drs.Message = "designation already exists with similar name";
                return _drs;
            }

            bool result = mst_designation.CreateDesignation(dbOperations, desg);

            if (result == false)
            {
                _drs.Status = false;
                _drs.Message = "unable to create designation";
                return _drs;
            }
            _drs.Status = true;
            _drs.Message = "Successful";
            return _drs;


        }

        public static DaoResponse UpdateDesignation(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            col_deparment desg = JsonConvert.DeserializeObject<col_deparment>(request!.Payload!)!;

            Dictionary<string, object> desgUpdate = new Dictionary<string, object>();
            desgUpdate.Add("name", desg.name!);
            DataTable dtb2 = dbOperations.ExecuteTable("select count(*) as ct from mst_designation where name=@name", new Dictionary<string, object>()
            {
                {"name", desg.name!}
            });
            if (Convert.ToInt32(dtb2.Rows[0]["ct"].ToString()) > 0)
            {
                _drs.Status = false;
                _drs.Message = "designation already exists with similar name";
                return _drs;
            }
            bool result = mst_designation.UpdateDesignation(dbOperations, desgUpdate, desg.id);
            if (result == false)
            {
                _drs.Status = false;
                _drs.Message = "unable to update designation";
            }
            _drs.Status = true;
            _drs.Message = "Successful";
            return _drs;

        }

    }
}


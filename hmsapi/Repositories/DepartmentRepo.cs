
using Newtonsoft.Json;
using System.Data;
using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;

namespace hmsapi.Repositories
{
    public class DepartmentRepo
    {
        public static DaoResponse GetAllDepartments(IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            List<Dictionary<string, object?>> deptList = new List<Dictionary<string, object?>>();
            mst_department.GetAllDepartments(dbOperations).ForEach(x => {

                DataTable dtb = dbOperations.ExecuteTable($"select user_name from mst_user where id='{x.reporting_manager}'", null);
                string reporting_manager = "NA";
                if (dtb.Rows.Count != 0)
                {
                    reporting_manager = $"{dtb.Rows[0]["user_name"]}";
                }

                Dictionary<string, object?> deptDict = new Dictionary<string, object?>();
                deptDict.Add("id", x.id);
                deptDict.Add("department", x.name);
                deptDict.Add("users", "NA");
                deptDict.Add("reporting_manager", reporting_manager);
                deptDict.Add("status", x.is_active == DateOnly.FromDateTime(new DateTime(2100, 12, 31)));


                deptList.Add(deptDict);

            });
            drs.Status = true;
            drs.Message = "Successful";
            drs.Payload = JsonConvert.SerializeObject(deptList);
            return drs;
        }
        public static DaoResponse CreateNewDepartment(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            col_deparment dept = JsonConvert.DeserializeObject<col_deparment>(request!.Payload!)!;
            dept.record_date = DateTime.Now;
            DataTable dtb = dbOperations.ExecuteTable("select count(*) as ct from mst_department where name=@name", new Dictionary<string, object>()
            {
                {"name", dept.name!}
            });
            if (dept.name == null || dept.reporting_manager == null)
            {
                throw new BadHttpRequestException("name or reporting manager required");
            }
            if (Convert.ToInt32(dtb.Rows[0]["ct"].ToString()) > 0)
            {
                _drs.Status = false;
                _drs.Message = "department already exists with similar name";
                return _drs;
            }


            bool result = mst_department.CreateDepartment(dbOperations, dept);

            if (result == false)
            {
                _drs.Status = false;
                _drs.Message = "unable to create department";
                return _drs;
            }
            _drs.Status = true;
            _drs.Message = "Successful";
            return _drs;


        }

        public static DaoResponse UpdateDepartment(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            col_deparment dept = JsonConvert.DeserializeObject<col_deparment>(request!.Payload!)!;

            Dictionary<string, object> deptUpdate = new Dictionary<string, object>();
            deptUpdate.Add("name", dept.name!);
            deptUpdate.Add("reporting_manager", dept.reporting_manager!);
            DataTable dtb = dbOperations.ExecuteTable("select count(*) as ct from mst_department where name=@name", new Dictionary<string, object>()
            {
                {"name", dept.name!}
            });
            if (Convert.ToInt32(dtb.Rows[0]["ct"].ToString()) > 0)
            {
                _drs.Status = false;
                _drs.Message = "department already exists with similar name";
                return _drs;
            }
            bool result = mst_department.UpdateDepartment(dbOperations, deptUpdate, dept.id);
            if (result == false)
            {
                _drs.Status = false;
                _drs.Message = "unable to update department";
            }
            _drs.Status = true;
            _drs.Message = "Successful";
            return _drs;

        }

    }
}


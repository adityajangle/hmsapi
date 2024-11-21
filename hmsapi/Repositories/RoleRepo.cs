using System;
using System.Data;
using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;
using Newtonsoft.Json;

namespace hmsapi.Repositories
{
	public class RoleRepo
	{

        public static DaoResponse GetAllRoles(IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            List<Dictionary<string, object?>> roleList = new List<Dictionary<string, object?>>();

            mst_role.GetAllRoles(dbOperations).ForEach(x =>
            {
                Dictionary<string, object?> roleDict = new Dictionary<string, object?>();
                roleDict.Add("role_id", x.id);
                roleDict.Add("role_name", x.name);
                roleDict.Add("users", "NA");
                roleDict.Add("status", x.is_active == DateOnly.FromDateTime(new DateTime(2100, 12, 31)));

                roleList.Add(roleDict);
            });


            drs.Status = true;
            drs.Message = "Successful";
            drs.Payload = JsonConvert.SerializeObject(roleList);
            return drs;
        }
        public static DaoResponse CreateNewRole(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            Dictionary<string, object> allDate = JsonConvert.DeserializeObject<Dictionary<string, object>>(request!.Payload!)!;

            col_role role = JsonConvert.DeserializeObject<col_role>(request!.Payload!)!;
            List<string> perms = JsonConvert.DeserializeObject<List<string>>(allDate["perms"].ToString()!)!; ;

            try
            {
                mst_role.CreateRole(dbOperations, role, perms);
            }catch(DataException dx)
            {
                _drs.Status = false;
                _drs.Message = dx.Message;
                return _drs;
            }
            _drs.Status = true;
            _drs.Message = "Successful";
            return _drs;
        }

        public static DaoResponse GetRoleById(string payload,IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            Dictionary<string, object> option = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload)!;

            DataTable dtb2 = dbOperations.ExecuteTable("select name from mst_role where is_active='2100-12-31' and id=@role_id", new Dictionary<string, object>() {
                {"role_id",option["role_id"]}
            });
            if (dtb2.Rows.Count == 0)
            {
                _drs.Status = false;
                _drs.Message = "no role found";
                return _drs;
            }

            string role_name = dtb2.Rows[0]["name"].ToString()!;


            DataTable dtb = dbOperations.ExecuteTable("select permission_id from ref_role_permission where role_id=@role_id and is_active='2100-12-31'", new Dictionary<string, object>() {
                {"role_id",option["role_id"]}
            });
            if(dtb.Rows.Count == 0) {
                _drs.Status = false;
                _drs.Message = "no role found";
                return _drs;
            }

            List<col_mst_permission> perms = new List<col_mst_permission>();

            foreach(DataRow dr in dtb.Rows)
            {

                col_mst_permission perm = new col_mst_permission();

                DataTable dtb1 = dbOperations.ExecuteTable("select * from mst_permission where id=@id and status='1'", new Dictionary<string, object>() {
                    {"id",dr["permission_id"]}
                });

                perm.category = dtb1.Rows[0]["category"].ToString()!;
                perm.status = (bool)dtb1.Rows[0]["status"];
                perm.name = dtb1.Rows[0]["name"].ToString()!;
                perm.id = dtb1.Rows[0]["id"].ToString()!;

                perms.Add(perm);
            }


            Dictionary<string, object> rtn = new Dictionary<string, object>();
            rtn.Add("role_name", role_name);
            rtn.Add("perms", perms);

            _drs.Status = true;
            _drs.Payload = JsonConvert.SerializeObject(rtn);
            _drs.Message = "role found";

            return _drs;
        }

        public static DaoResponse UpdateRole(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            col_role role = JsonConvert.DeserializeObject<col_role>(request!.Payload!)!;

            Dictionary<string, object> roleUpdate = new Dictionary<string, object>();
            roleUpdate.Add("name", role.name!);
            
            bool result = mst_department.UpdateDepartment(dbOperations, roleUpdate, role.id);
            if (result == false)
            {
                _drs.Status = false;
                _drs.Message = "unable to update role";
                return _drs;
            }
            _drs.Status = true;
            _drs.Message = "Successful";
            return _drs;
        }


        
    }
}


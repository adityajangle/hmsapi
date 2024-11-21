
using Newtonsoft.Json;
using System.Data;
using hmsapi.Data;
using hmsapi.Models;

namespace hmsapi.Repositories
{
    public class PermissionRepo
    {
        public static DaoResponse GetAllPermisions(IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            List<col_mst_permission?> permList = mst_Permission.GetAllPermissions(dbOperations);
            drs.Status = true;
            drs.Message = "Successful";
            drs.Payload = JsonConvert.SerializeObject(permList);
            return drs;
        }

        public static bool CheckUserHavePermission(IDbOperations dbOperations, string permissionID, string uid)
        {
            DataTable dtb = dbOperations.ExecuteTable($"select p.id from mst_user u join ref_user_role ur on ur.user_id = u.userid join ref_role_permission rp on ur.role_id = rp.role_id join mst_permission p on p.id = rp.permission_id where u.userid='{uid}' and u.is_active='2100-12-31' and ur.is_active='2100-12-31'", null);
            List<string> perms = new List<string>();
            foreach (DataRow dr in dtb.Rows)
            {
                perms.Add(dr["id"].ToString()!);
            }

            return perms.Where(x => x == permissionID).Any();
        }

        public static bool CheckUserBelongToSubTask(IDbOperations dbOperations, string mid, string sid, string uid)
        {
            DataTable dtb = dbOperations.ExecuteTable($"" +
                $"select * from tbl_sub_task where tbl_main_task_id='{mid}' and " +
                $"id={sid} and user_id='{uid}'", null);
            return dtb.Rows.Count != 0;
        }

        public static bool CheckUserIsReportingManager(IDbOperations dbOperations, string deptid, string uid)
        {
            DataTable dtb = dbOperations.ExecuteTable($"select * from mst_department d join mst_user u on u.userid = d.reporting_manager where d.id='{deptid}' and u.userif={uid}", null);
            return dtb.Rows.Count != 0;
        }

        public static bool CheckUserBelongToMSTDepartment(IDbOperations dbOperations, string mid, string uid)
        {
            DataTable dtb = dbOperations.ExecuteTable($"select * from tbl_main_task m join mst_department d on d.id = m.department_id join mst_user u on u.deptid = d.id where u.userid='{uid}' and m.id='{mid}'", null);
            return dtb.Rows.Count != 0;
        }

    }
}


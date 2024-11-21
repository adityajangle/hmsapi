using System;
using System.Data;
using System.Net.Mail;
using hmsapi.Services;
using hmsapi.Data;
using hmsapi.Services;

namespace hmsapi.Data
{
    public class DbStructure
    {
        public static void Init(IDbOperations dbOperations)
        {
            CheckTblUserSystemAdmin(dbOperations);
            CheckTblPermission(dbOperations);
            CheckTblDesignationt(dbOperations);
            CheckTblDepartment(dbOperations);

        }

        private static void CheckTblPermission(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_permission", null);
            if (dtb.Rows.Count == 0)
            {
                mst_Permission.BulkInsertTblPermission(_dbOperations);
            }
        }

        private static void CheckTblDesignationt(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_designation where id='9999'", null);
            if (dtb.Rows.Count == 0)
            {
                string query = $"INSERT INTO mst_designation(id,name,created_date)VALUES('9999','System admin','{UtilService.DtMSql(DateTime.Now)}')";
                _dbOperations.ExecuteTable(query, null);
            }
        }

        private static void CheckTblDepartment(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_department where id='9999'", null);
            if (dtb.Rows.Count == 0)
            {
                string query = $"INSERT INTO mst_department(id,name,record_date)VALUES('9999','DEPT-NOTAVAILABLE','{UtilService.DtMSql(DateTime.Now)}')";
                _dbOperations.ExecuteTable(query, null);
            }
        }

        private static void CheckTblUserSystemAdmin(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_user where id='9999'", null);
            if (dtb.Rows.Count == 0)
            {
                string query = "INSERT INTO mst_user(id,user_name,employee_code,address,state,city,pincode,mobile,email,birthdate,did,deptid,password,joining_date,last_login,gender,work_start,work_end)" +
                    $"VALUES('9999','System Admin','9999','','','','','1234567890','system@smp.com','2100-12-31','9999','9999','{BCrypt.Net.BCrypt.HashPassword("QWERTY@123")}','{UtilService.DtMSql(DateTime.Now)}','{UtilService.DtMSql(DateTime.Now)}','N','{TimeOnly.FromDateTime(DateTime.Now)}','{TimeOnly.FromDateTime(DateTime.Now)}')";
                _dbOperations.ExecuteTable(query, null);
            }
        }

    }


}


using System;
using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;
using hmsapi.Services;
using Newtonsoft.Json;
using System.Data;

namespace hmsapi.Repositories
{
	public class UserRepo
	{
        public static DaoResponse GetAllUsers(IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            List<Dictionary<string, object?>> userList = new List<Dictionary<string, object?>>();

            mst_user.GetAllUsers(dbOperations).ForEach(x =>
            {
                DataTable dtb = dbOperations.ExecuteTable($"select name from mst_department where id='{x.deptid}'", null);
                string departmentName = "";
                if (dtb.Rows.Count != 0)
                {
                    departmentName = dtb.Rows[0]["name"].ToString() ?? "";
                }

                DataTable dtb1 = dbOperations.ExecuteTable($"select name from mst_designation where id='{x.did}'", null);
                string designationName = "";
                if (dtb1.Rows.Count != 0)
                {
                    designationName = dtb.Rows[0]["name"].ToString() ?? "";
                }

                Dictionary<string, object?> userDict = new Dictionary<string, object?>();
                userDict.Add("id", x.id);
                userDict.Add("user_name", x.user_name);
                userDict.Add("email", x.email);
                userDict.Add("department", departmentName);
                userDict.Add("designation", designationName);
                userDict.Add("status", x.is_active == DateOnly.FromDateTime(new DateTime(2100, 12, 31)));

                userList.Add(userDict);
            });

            //DesignationRepo.GetAllDesignations(dbOperations);
            drs.Status = true;
            drs.Message = "Successful";
            drs.Payload = JsonConvert.SerializeObject(userList);
            return drs;
        }

        public static DaoResponse GetUserById(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            Dictionary<string, object> user = JsonConvert.DeserializeObject<Dictionary<string, object>>(request!.Payload!)!;

            DataTable dtb = dbOperations.ExecuteTable("select * from mst_user where userid=@userid and id !='9999' and is_active='2100-12-31'", new Dictionary<string, object>() {
                {"id",user["id"]}
            });

            if (dtb.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "No such user found..!";
                return drs;
            }

            Dictionary<string, object?> userDict = new Dictionary<string, object?>();

            userDict.Add("user_name", dtb.Rows[0]["user_name"]);
            userDict.Add("employee_code", dtb.Rows[0]["employee_code"]);
            userDict.Add("address", dtb.Rows[0]["address"]);
            userDict.Add("state", dtb.Rows[0]["state"]);
            userDict.Add("city", dtb.Rows[0]["city"]);
            userDict.Add("pincode", dtb.Rows[0]["pincode"]);
            userDict.Add("mobile", dtb.Rows[0]["mobile"]);
            userDict.Add("gender", dtb.Rows[0]["gender"]);
            userDict.Add("email", dtb.Rows[0]["email"]);
            userDict.Add("deptid", dtb.Rows[0]["deptid"]);
            userDict.Add("did", dtb.Rows[0]["did"]);
            userDict.Add("birthdate", dtb.Rows[0]["birthdate"]);
            userDict.Add("joining_date", dtb.Rows[0]["joining_date"]);

            DataTable dtb1 = dbOperations.ExecuteTable("select * from ref_user_role where id=@id and is_active='2100-12-31'", new Dictionary<string, object>() {
                {"id",user["id"]}
            });

            if (dtb1.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "user role not found";
                return drs;
            }

            userDict.Add("role_id", dtb1.Rows[0]["role_id"].ToString());

            drs.Status = true;
            drs.Message = "Successful";
            drs.Payload = JsonConvert.SerializeObject(userDict);
            return drs;
        }


        public static DaoResponse CreateNewUser(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            Dictionary<string, object> allDate = JsonConvert.DeserializeObject<Dictionary<string, object>>(request!.Payload!)!;

            col_mst_user user = JsonConvert.DeserializeObject<col_mst_user>(request!.Payload!)!;
            string roleId = (string)allDate["role_id"];
            user.password = BCrypt.Net.BCrypt.HashPassword("ABC123");
            user.last_login = DateTime.Now;
            try
            {
                mst_user.CreateUser(dbOperations, user, roleId);
            }
            catch (DataException dx)
            {
                _drs.Status = false;
                _drs.Message = dx.Message;
                return _drs;
            }
            _drs.Status = true;
            _drs.Message = "Successful";
            return _drs;


        }



        public static DaoResponse UpdateUser(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            Dictionary<string, object> allDate = JsonConvert.DeserializeObject<Dictionary<string, object>>(request!.Payload!)!;

            col_mst_user user = JsonConvert.DeserializeObject<col_mst_user>(request!.Payload!)!;
            string roleId = (string)allDate["role_id"];

            Dictionary<string, object> userUpdate = new Dictionary<string, object>();
            userUpdate.Add("address", user.address!);
            userUpdate.Add("birthdate", user.birthdate);
            userUpdate.Add("city", user.city!);
            userUpdate.Add("deptid", user.deptid!);
            userUpdate.Add("did", user.did!);
            userUpdate.Add("email", user.email!);
            userUpdate.Add("employee_code", user.employee_code!);
            userUpdate.Add("mobile", user.mobile!);
            userUpdate.Add("pincode", user.pincode!);
            userUpdate.Add("state", user.state!);
            userUpdate.Add("user_name", user.user_name!);

            try
            {
                mst_user.UpdateUser(dbOperations, userUpdate, user.id.ToString(), roleId);
            }
            catch (DataException dx)
            {
                _drs.Status = false;
                _drs.Message = dx.Message;
                return _drs;
            }
            _drs.Status = true;
            _drs.Message = "Successful";
            return _drs;

        }


        public static DaoResponse ChangePassword(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            DaoPasswordChange daoLogin = JsonConvert.DeserializeObject<DaoPasswordChange>(request!.Payload!)!;
            DataTable dtb = dbOperations.ExecuteTable("select * from mst_user where employee_code=@employee_code  and is_active='2100-12-31'", new Dictionary<string, object>() {
                {"employee_code",daoLogin.employee_code!}
            });
            if (dtb.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "No such user found..!";
                return drs;
            }
            if (!BCrypt.Net.BCrypt.Verify(daoLogin.old_pwd, dtb.Rows[0]["password"].ToString()))
            {
                drs.Status = false;
                drs.Message = "Credentials Invalid";
                return drs;
            };
            int upd = dbOperations.ExecuteUpdate($"update mst_user set password='{BCrypt.Net.BCrypt.HashPassword(daoLogin.new_pwd)}' where employee_code=@employee_code and is_active='2100-12-31'", new Dictionary<string, object>() {
                {"employee_code",daoLogin.employee_code!}
            });

            if (upd == 0)
            {
                drs.Status = false;
                drs.Message = "unable to change password";
                return drs;
            }
            drs.Status = true;
            drs.Message = "password updated successfully";
            return drs;
        }


        public static DaoResponse LoginUser(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            DaoLogin daoLogin = JsonConvert.DeserializeObject<DaoLogin>(request!.Payload!)!;
            DataTable dtb = dbOperations.ExecuteTable("select * from mst_user where employee_code=@employee_code  and is_active='2100-12-31'", new Dictionary<string, object>() {
                {"employee_code",daoLogin.employee_code!}
            });

            if (dtb.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "No such user found..!";
                return drs;
            }

            if (!BCrypt.Net.BCrypt.Verify(daoLogin.pwd, dtb.Rows[0]["password"].ToString()))
            {
                drs.Status = false;
                drs.Message = "Credentials Invalid";
                return drs;
            };

            if (_sessionManager.AuthCaptcha != daoLogin.captch)
            {
                drs.Status = false;
                drs.Message = "Credentials captcha";
                return drs;
            }

            Dictionary<string, object?> userDict = new Dictionary<string, object?>();
            userDict.Add("user_name", dtb.Rows[0]["user_name"]);
            userDict.Add("employee_code", dtb.Rows[0]["employee_code"]);
            userDict.Add("mobile", dtb.Rows[0]["mobile"]);
            userDict.Add("email", dtb.Rows[0]["email"]);
            userDict.Add("userid", dtb.Rows[0]["userid"]);

            DataTable deptDtb = dbOperations.ExecuteTable($"select name from mst_department where is_active='2100-12-31' and id='{dtb.Rows[0]["deptid"]}'", null);
            if (deptDtb.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "user department not found";
                return drs;
            }

            DataTable desgDtb = dbOperations.ExecuteTable($"select name from mst_designation where is_active='2100-12-31' and id='{dtb.Rows[0]["did"]}'", null);
            if (desgDtb.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "user designation not found";
                return drs;
            }

            userDict.Add("department", deptDtb.Rows[0]["name"]);
            userDict.Add("designation", desgDtb.Rows[0]["name"]);
            userDict.Add("last_login", dtb.Rows[0]["last_login"]);

            dbOperations.ExecuteUpdate($"update mst_user set last_login='{UtilService.DttMSql(DateTime.Now)}' where employee_code=@employee_code and is_active='2100-12-31'", new Dictionary<string, object>() {
                {"employee_code",daoLogin.employee_code!}
            });

            if (dtb.Rows[0]["userid"].ToString() == "9999")
            {
                drs.Status = true;
                drs.Message = "Successful";
                drs.Payload = JsonConvert.SerializeObject(userDict);
                return drs;
            }

            DataTable dtb1 = dbOperations.ExecuteTable("select * from ref_user_role where user_id=@user_id and is_active='2100-12-31'", new Dictionary<string, object>() {
                {"user_id",dtb.Rows[0]["userid"]}
            });

            if (dtb1.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "user role not found";
                return drs;
            }





            DaoResponse _roleRepo = RoleRepo.GetRoleById(JsonConvert.SerializeObject(new Dictionary<string, object>()
            {
                { "role_id",dtb1.Rows[0]["role_id"].ToString()!}
            }), dbOperations);

            if (_roleRepo.Status == false)
            {
                return _roleRepo;
            }
            userDict.Add("perms", _roleRepo.Payload);

            drs.Status = true;
            drs.Message = "Successful";
            drs.Payload = JsonConvert.SerializeObject(userDict);
            return drs;
        }
    }
}


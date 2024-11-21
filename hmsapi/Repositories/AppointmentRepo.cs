using System;
using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;
using Newtonsoft.Json;
using System.Data;

namespace hmsapi.Repositories
{
	public class AppointmentRepo
	{
        public static DaoResponse CreateNewAppointment(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            Dictionary<string, object> user = JsonConvert.DeserializeObject<Dictionary<string, object>>(request!.Payload!)!;

            DataTable dtb = dbOperations.ExecuteTable("select * from mst_patient where userid=@userid and id !='9999' and is_active='2100-12-31'", new Dictionary<string, object>() {
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
        public static DaoResponse GetAllAppointments(IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            List<Dictionary<string, object?>> aptList = new List<Dictionary<string, object?>>();
            pat_appointment.GetAllAppointments(dbOperations).ForEach(x => {

                Dictionary<string, object?> aptDict = new Dictionary<string, object?>();
                aptDict.Add("id", x.id);
                aptDict.Add("appointment_date", x.appointment_date);
                aptDict.Add("appointment_reason", x.appointment_reason);
                aptDict.Add("appointment_status", x.appointment_status);
                aptDict.Add("appointment_time", x.appointment_time);
                aptDict.Add("appointment_type", x.appointment_type);
                aptDict.Add("created_at", x.created_at);
                aptDict.Add("doctor_id", x.doctor_id);
                aptDict.Add("patient_id", x.patient_id);
                aptDict.Add("booked_id", x.booked_id);
                aptDict.Add("status", x.is_active == DateOnly.FromDateTime(new DateTime(2100, 12, 31)));


                aptList.Add(aptDict);

            });
            drs.Status = true;
            drs.Message = "Successful";
            drs.Payload = JsonConvert.SerializeObject(aptList);
            return drs;
        }
    }
    
}


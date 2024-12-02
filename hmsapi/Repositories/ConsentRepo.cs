using hmsapi.Data;
using hmsapi.Managers;
using hmsapi.Models;
using Newtonsoft.Json;
using System.Data;

namespace hmsapi.Repositories
{
    public class ConsentRepo
    {
        public static DaoResponse GetConsentList(IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();


            List<col_mst_consent> consentList = mst_consent.GetAllConsentType(dbOperations)!;
            //DesignationRepo.GetAllDesignations(dbOperations);
            drs.Status = true;
            drs.Message = "Successful";
            drs.Payload = JsonConvert.SerializeObject(consentList);
            return drs;
        }
        public static DaoResponse CreateNewConsent(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse _drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;

             col_pat_consent consent = JsonConvert.DeserializeObject<col_pat_consent>(request!.Payload!)!;
            try
            {
                mst_consent.CreateConsent(dbOperations,consent);
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
        public static DaoResponse GetPatientConsentList(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            Dictionary<string, object> pat_consent = JsonConvert.DeserializeObject<Dictionary<string, object>>(request!.Payload!)!;
            List<Dictionary<string, object?>> pat_consentList = new List<Dictionary<string, object?>>();

            DataTable dtb = dbOperations.ExecuteTable("select * from pat_consent where patient_id=@id", new Dictionary<string, object>() {
                {"id",pat_consent["patient_id"]}
            });

            if (dtb.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "Consent Not Found....!";
                return drs;
            }
            else
            {
                mst_consent.GetAllPat_Consent(pat_consent["patient_id"].ToString()!, dbOperations).ForEach(x =>
                {
                    DataTable dtb = dbOperations.ExecuteTable($"select patient_name from mst_patient where id='{x.patient_id}'", null);
                    string patientName = "";
                    if (dtb.Rows.Count != 0)
                    {
                        patientName = dtb.Rows[0]["patient_name"].ToString() ?? "";
                    }

                    dtb = dbOperations.ExecuteTable($"select type from mst_consent where mst_consent_id='{x.mst_consent_id}'", null);
                    string consentType = "";
                    if (dtb.Rows.Count != 0)
                    {
                        consentType = dtb.Rows[0]["type"].ToString() ?? "";
                    }

                    dtb = dbOperations.ExecuteTable($"select name from mst_department where id='{x.department_id}'", null);
                    string departmentName = "";
                    if (dtb.Rows.Count != 0)
                    {
                        departmentName = dtb.Rows[0]["name"].ToString() ?? "";
                    }
                    dtb = dbOperations.ExecuteTable($"select user_name from mst_user where id='{x.interpreted_by}'", null);
                    string interpretedby = "";
                    if (dtb.Rows.Count != 0)
                    {
                        interpretedby = dtb.Rows[0]["user_name"].ToString() ?? "";
                    }
                    dtb = dbOperations.ExecuteTable($"select user_name from mst_user where id='{x.doctor_id}'", null);
                    string doctorName = "";
                    if (dtb.Rows.Count != 0)
                    {
                        doctorName = dtb.Rows[0]["user_name"].ToString() ?? "";
                    }
                    dtb = dbOperations.ExecuteTable($"select user_name from mst_user where id='{x.created_by}'", null);
                    string createdName = "";
                    if (dtb.Rows.Count != 0)
                    {
                        createdName = dtb.Rows[0]["user_name"].ToString() ?? "";
                    }

                    Dictionary<string, object?> consentDict = new Dictionary<string, object?>();
                    consentDict.Add("consent_id", x.consent_id);
                    consentDict.Add("patient_id", x.patient_id);
                    consentDict.Add("patient_name", patientName);
                    consentDict.Add("mst_consent_id", x.mst_consent_id);
                    consentDict.Add("consent_type", consentType);
                    consentDict.Add("department_id", x.department_id);
                    consentDict.Add("deaprtment_name", departmentName);
                    consentDict.Add("interpreted_by", x.interpreted_by);
                    consentDict.Add("interpreted_by_name", interpretedby);
                    consentDict.Add("doctor_id", x.doctor_id);
                    consentDict.Add("doctor_name", doctorName);
                    consentDict.Add("witness_one_name", x.witness_one_name);
                    consentDict.Add("witness_one_rel", x.witness_one_rel);
                    consentDict.Add("witness_two_name", x.witness_two_name);
                    consentDict.Add("witness_two_rel", x.witness_two_rel);
                    consentDict.Add("created_date", x.created_date);
                    consentDict.Add("created_by", x.created_by);
                    consentDict.Add("created_name", createdName);
                    pat_consentList.Add(consentDict);
                });

                //DesignationRepo.GetAllDesignations(dbOperations);
                drs.Status = true;
                drs.Message = "Successful";
                drs.Payload = JsonConvert.SerializeObject(pat_consentList);
                return drs;
            }
            
        }

        public static DaoResponse GetPatientConsentListByAppointment(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            Dictionary<string, object> pat_consent = JsonConvert.DeserializeObject<Dictionary<string, object>>(request!.Payload!)!;
            List<Dictionary<string, object?>> pat_consentList = new List<Dictionary<string, object?>>();

            DataTable dtb = dbOperations.ExecuteTable("select * from pat_consent where patient_id=@id and appointment_id=@id1", new Dictionary<string, object>() {
                {"id",pat_consent["patient_id"]},{ "id1",pat_consent["appointment_id"]}
            });

            if (dtb.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "Consent Not Found....!";
                return drs;
            }
            else
            {
                mst_consent.GetAllPatConsentByAppointment(pat_consent["patient_id"].ToString()!, pat_consent["appointment_id"].ToString(), dbOperations).ForEach(x =>
                {
                    DataTable dtb = dbOperations.ExecuteTable($"select patient_name from mst_patient where id='{x.patient_id}'", null);
                    string patientName = "";
                    if (dtb.Rows.Count != 0)
                    {
                        patientName = dtb.Rows[0]["patient_name"].ToString() ?? "";
                    }

                    dtb = dbOperations.ExecuteTable($"select type from mst_consent where mst_consent_id='{x.mst_consent_id}'", null);
                    string consentType = "";
                    if (dtb.Rows.Count != 0)
                    {
                        consentType = dtb.Rows[0]["type"].ToString() ?? "";
                    }

                    dtb = dbOperations.ExecuteTable($"select name from mst_department where id='{x.department_id}'", null);
                    string departmentName = "";
                    if (dtb.Rows.Count != 0)
                    {
                        departmentName = dtb.Rows[0]["name"].ToString() ?? "";
                    }
                    dtb = dbOperations.ExecuteTable($"select user_name from mst_user where id='{x.interpreted_by}'", null);
                    string interpretedby = "";
                    if (dtb.Rows.Count != 0)
                    {
                        interpretedby = dtb.Rows[0]["user_name"].ToString() ?? "";
                    }
                    dtb = dbOperations.ExecuteTable($"select user_name from mst_user where id='{x.doctor_id}'", null);
                    string doctorName = "";
                    if (dtb.Rows.Count != 0)
                    {
                        doctorName = dtb.Rows[0]["user_name"].ToString() ?? "";
                    }
                    dtb = dbOperations.ExecuteTable($"select user_name from mst_user where id='{x.created_by}'", null);
                    string createdName = "";
                    if (dtb.Rows.Count != 0)
                    {
                        createdName = dtb.Rows[0]["user_name"].ToString() ?? "";
                    }

                    Dictionary<string, object?> consentDict = new Dictionary<string, object?>();
                    consentDict.Add("consent_id", x.consent_id);
                    consentDict.Add("patient_id", x.patient_id);
                    consentDict.Add("patient_name", patientName);
                    consentDict.Add("mst_consent_id", x.mst_consent_id);
                    consentDict.Add("consent_type", consentType);
                    consentDict.Add("department_id", x.department_id);
                    consentDict.Add("deaprtment_name", departmentName);
                    consentDict.Add("interpreted_by", x.interpreted_by);
                    consentDict.Add("interpreted_by_name", interpretedby);
                    consentDict.Add("doctor_id", x.doctor_id);
                    consentDict.Add("doctor_name", doctorName);
                    consentDict.Add("witness_one_name", x.witness_one_name);
                    consentDict.Add("witness_one_rel", x.witness_one_rel);
                    consentDict.Add("witness_two_name", x.witness_two_name);
                    consentDict.Add("witness_two_rel", x.witness_two_rel);
                    consentDict.Add("created_date", x.created_date);
                    consentDict.Add("created_by", x.created_by);
                    consentDict.Add("created_name", createdName);
                    pat_consentList.Add(consentDict);
                });

                //DesignationRepo.GetAllDesignations(dbOperations);
                drs.Status = true;
                drs.Message = "Successful";
                drs.Payload = JsonConvert.SerializeObject(pat_consentList);
                return drs;
            }

        }


        public static DaoResponse GetConsentForm(SessionManager _sessionManager, IDbOperations dbOperations)
        {
            DaoResponse drs = new DaoResponse();
            DaoRequest request = _sessionManager.RequestData;
            Dictionary<string, object> mst_consenttype = JsonConvert.DeserializeObject<Dictionary<string, object>>(request!.Payload!)!;

           // mst_consent.AddConsent(dbOperations);

            DataTable dtb = dbOperations.ExecuteTable("select * from pat_consent where consent_id=@id", new Dictionary<string, object>() {
                {"id",mst_consenttype["consent_id"]}
            });

            if (dtb.Rows.Count == 0)
            {
                drs.Status = false;
                drs.Message = "Consent Not Found....!";
                return drs;
            }
            else
            {
                string consent_form = mst_consent.GetConsentForm(mst_consenttype["consent_id"].ToString()!, dbOperations);
                drs.Status = true;
                drs.Message = "Successful";
                drs.Payload = consent_form;
                return drs;
            }

        }

    }
}

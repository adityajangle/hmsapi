using System;
using hmsapi.Services;
using System.Data;

namespace hmsapi.Data
{
	public class col_pat_appointment
	{
        public string? id { get; set; }
        public string? patient_id { get; set; }
        public DateOnly appointment_date { get; set; }
        public TimeOnly appointment_time { get; set; }
        public string? appointment_status { get; set; }
        public string? appointment_type { get; set; }
        public string? created_at { get; set; }
        public string? appointment_reason { get; set; }
        public string? doctor_id { get; set; }
        public string? booked_id { get; set; }
        public DateOnly is_active { get; set; }
    }

    public class pat_appointment
    {
        public static void CreateAppointment(IDbOperations dbOperations, col_mst_user col, string roleId)
        {
            List<string> _columns = new List<string>();
            col.GetType().GetProperties().ToList().ForEach(
                x => _columns.Add(x.Name));
            _columns.Remove("id");
            _columns.Remove("is_active");
            string query = $"insert into pat_appointment({string.Join(',', _columns.ToArray())})values( {string.Join(',', _columns.Select(x => $"@{x}").ToArray())})";
            dbOperations.ExecuteNonTransaction((command) => {
                dbOperations.AddCommandParams(col, command);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new DataException("Unable to add user reference");
                }
            }, query);
        }

        public static List<col_pat_appointment?> GetAllAppointments(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from pat_appointment", null);
            List<col_pat_appointment?> aptList = new List<col_pat_appointment?>();
            foreach (DataRow x in dtb.Rows)
            {
                col_pat_appointment? apt = new col_pat_appointment();
                apt = _dbOperations.SelectParsedData(apt, x);
                aptList.Add(apt);
            }
            return aptList;
        }
    }
}


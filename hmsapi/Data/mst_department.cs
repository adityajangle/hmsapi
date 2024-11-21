using System;
using System.Data;

namespace hmsapi.Data
{
	public class col_deparment {
        public string? id { get; set; }
        public string? name { get; set; }
        public DateTime record_date { get; set; }
        public DateOnly is_active { get; set; }
        public int? reporting_manager { get; set; }

    }

	public class mst_department
	{

		public static bool CreateDepartment(IDbOperations dbOperations, col_deparment col)
		{
            bool _result = false;
            List<string> _columns = new List<string>();
            col.GetType().GetProperties().ToList().ForEach(
                x => _columns.Add(x.Name));
            _columns.Remove("id");
            _columns.Remove("is_active");
            string query = $"insert into mst_department({string.Join(',', _columns.ToArray())})values( {string.Join(',', _columns.Select(x => $"@{x}").ToArray())})";
            dbOperations.ExecuteNonTransaction((command) => {
                dbOperations.AddCommandParams(col, command);
                _result = command.ExecuteNonQuery() != 0;
            }, query);
            return _result;
        }

        public static List<col_deparment?> GetAllDepartments(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_department", null);
            List<col_deparment?> deptList = new List<col_deparment?>();
            foreach (DataRow x in dtb.Rows)
            {

                col_deparment? dept = new col_deparment();

                dept = _dbOperations.SelectParsedData(dept, x);
                deptList.Add(dept);
            }
            return deptList;
        }

        public static bool UpdateDepartment(IDbOperations _dbOperations, Dictionary<string, object> data, string id)
        {
            bool _result = false;
            List<string> toUpdate = new List<string>();
            foreach (var x in data)
            {
                toUpdate.Add($"{x.Key}=@{x.Key}");
            }

            string query = $"update mst_department set {string.Join(',', toUpdate)}  where id=@id";
            _dbOperations.ExecuteNonTransaction((command) => {

                _dbOperations.AddCmdParams(data, command);

                command.Parameters.AddWithValue($"@id", id);
                _result = command.ExecuteNonQuery() != 0;

            }, query);
            return _result;
        }
    }
}


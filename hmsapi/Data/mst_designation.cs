using System;
using System.Data;
using hmsapi.Data;

namespace hmsapi.Data
{
	public class col_designation {
        public int id { get; set; }
        public string? name { get; set; }
        public DateOnly is_active { get; set; }
        public DateTime created_date { get; set; }
    }


	public class mst_designation
	{
        public static bool CreateDesignation(IDbOperations dbOperations, col_designation col)
        {
            bool _result = false;
            List<string> _columns = new List<string>();
            col.GetType().GetProperties().ToList().ForEach(
                x => _columns.Add(x.Name));
            _columns.Remove("id");
            _columns.Remove("is_active");
            string query = $"insert into mst_designation({string.Join(',', _columns.ToArray())})values( {string.Join(',', _columns.Select(x => $"@{x}").ToArray())})";
            dbOperations.ExecuteNonTransaction((command) => {
                dbOperations.AddCommandParams(col, command);
                _result = command.ExecuteNonQuery() != 0;
            }, query);
            return _result;
        }

        public static List<col_designation?> GetAllDesignations(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_designation", null);
            List<col_designation?> designationList = new List<col_designation?>();
            foreach (DataRow x in dtb.Rows)
            {

                col_designation? designation = new col_designation();
                designation = _dbOperations.SelectParsedData(designation, x);
                designationList.Add(designation);
            }
            return designationList;
        }

        public static bool UpdateDesignation(IDbOperations _dbOperations, Dictionary<string, object> data, string id)
        {
            bool _result = false;
            List<string> toUpdate = new List<string>();
            foreach (var x in data)
            {
                toUpdate.Add($"{x.Key}=@{x.Key}");
            }

            string query = $"update mst_designation set {string.Join(',', toUpdate)}  where id=@id";
            _dbOperations.ExecuteNonTransaction((command) => {

                _dbOperations.AddCmdParams(data, command);

                command.Parameters.AddWithValue($"@id", id);
                _result = command.ExecuteNonQuery() != 0;

            }, query);
            return _result;
        }
    }
}


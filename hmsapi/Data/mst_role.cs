using System;
using System.Data;
using hmsapi.Services;

namespace hmsapi.Data
{
	public class col_role {
		public string? id { get; set; }
		public string? name { get; set; }
		public string? description { get; set; }
		public DateTime created_date { get; set; }
		public DateOnly is_active { get; set; }

	}

	public class mst_role
	{
        public static void CreateRole(IDbOperations dbOperations, col_role col,List<string> perms)
        {
            
            List<string> _columns = new List<string>();
            col.GetType().GetProperties().ToList().ForEach(
                x => _columns.Add(x.Name));
            _columns.Remove("id");
            _columns.Remove("is_active");
            string query = $"insert into mst_role({string.Join(',', _columns.ToArray())})values( {string.Join(',', _columns.Select(x => $"@{x}").ToArray())})";
            dbOperations.ExecuteNonTransaction((command) => {
                dbOperations.AddCommandParams(col, command);
                if(command.ExecuteNonQuery() == 0)
                {
                    throw new DataException("Unable to add role");
                }
                _columns = new List<string>();
                ref_role_permission perm = new ref_role_permission();
                perm.GetType().GetProperties().ToList().ForEach(
                x => _columns.Add(x.Name));
                _columns.Remove("is_active");
                perms.ForEach(permobj =>
                {
                    string query = $"insert into ref_role_permission({string.Join(',', _columns.ToArray())})values( {string.Join(',', _columns.Select(x => $"@{x}").ToArray())})";
                    dbOperations.ExecuteNonTransaction((cmd) =>
                    {
                        cmd.Parameters.AddWithValue("role_id",command.LastInsertedId.ToString());
                        cmd.Parameters.AddWithValue("permission_id", permobj);
                        cmd.Parameters.AddWithValue("created_date", UtilService.DtMSql(DateTime.Now));
                        if(cmd.ExecuteNonQuery() == 0)
                        {
                            throw new DataException("Unable to add role_permission reference");
                        }
                    }, query);
                });
            }, query);
           
        }

        public static List<col_role?> GetAllRoles(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_role", null);
            List<col_role?> roleList = new List<col_role?>();
            foreach (DataRow x in dtb.Rows)
            {

                col_role? role = new col_role();

                role = _dbOperations.SelectParsedData(role, x);
                roleList.Add(role);
            }
            return roleList;
        }

        public static bool UpdateRole(IDbOperations _dbOperations, Dictionary<string, object> data, int userid)
        {
            bool _result = false;
            List<string> toUpdate = new List<string>();
            foreach (var x in data)
            {
                toUpdate.Add($"{x.Key}=@{x.Key}");
            }

            string query = $"update mst_role set {string.Join(',', toUpdate)}  where id=@id";
            _dbOperations.ExecuteNonTransaction((command) => {

                _dbOperations.AddCmdParams(data, command);

                command.Parameters.AddWithValue($"@id", userid);
                _result = command.ExecuteNonQuery() != 0;

            }, query);
            return _result;
        }
    }
}


using System;
using hmsapi.Services;
using System.Data;

namespace hmsapi.Data
{
	public class col_mst_user
	{
        public string? id { get; set; }
        public string? user_name { get; set; }
        public string? employee_code { get; set; }
        public string? address { get; set; }
        public string? state { get; set; }
        public string? city { get; set; }
        public string? pincode { get; set; }
        public string? mobile { get; set; }
        public string? email { get; set; }
        public DateOnly birthdate { get; set; }
        public string? did { get; set; }
        public string? deptid { get; set; }
        public string? password { get; set; }
        public DateOnly joining_date { get; set; }
        public DateOnly is_active { get; set; }
        public DateTime last_login { get; set; }
        public string? gender { get; set; }
        public TimeOnly work_start { get; set; }
        public TimeOnly work_end { get; set; }
    }
	public class mst_user
	{
        public static void CreateUser(IDbOperations dbOperations, col_mst_user col, string roleId)
        {
            List<string> _columns = new List<string>();
            col.GetType().GetProperties().ToList().ForEach(
                x => _columns.Add(x.Name));
            _columns.Remove("id");
            _columns.Remove("is_active");
            string query = $"insert into mst_user({string.Join(',', _columns.ToArray())})values( {string.Join(',', _columns.Select(x => $"@{x}").ToArray())})";
            dbOperations.ExecuteNonTransaction((command) => {
                dbOperations.AddCommandParams(col, command);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new DataException("Unable to add user reference");
                }

                query = $"insert into ref_user_role (user_id,role_id,created_date)values(@user_id,@role_id,@created_date)";
                dbOperations.ExecuteNonTransaction((cmd) =>
                {
                    cmd.Parameters.AddWithValue("user_id", command.LastInsertedId.ToString());
                    cmd.Parameters.AddWithValue("role_id", roleId);
                    cmd.Parameters.AddWithValue("created_date", UtilService.DtMSql(DateTime.Now));
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        throw new DataException("Unable to add user_role reference");
                    }
                }, query);
            }, query);
        }

        public static List<col_mst_user?> GetAllUsers(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_user where id<>'9999'", null);
            List<col_mst_user?> userList = new List<col_mst_user?>();
            foreach (DataRow x in dtb.Rows)
            {
                col_mst_user? user = new col_mst_user();
                user = _dbOperations.SelectParsedData(user, x);
                userList.Add(user);
            }
            return userList;
        }

        public static void UpdateUser(IDbOperations _dbOperations, Dictionary<string, object> data, string id, string roleId)
        {

            List<string> toUpdate = new List<string>();
            foreach (var x in data)
            {
                toUpdate.Add($"{x.Key}=@{x.Key}");
            }
            string query = $"update mst_user set {string.Join(',', toUpdate)}  where id=@id";
            _dbOperations.ExecuteNonTransaction((command) => {
                _dbOperations.AddCmdParams(data, command);
                command.Parameters.AddWithValue($"@id", id);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new DataException("Unable to update user");
                }
            }, query);

            DataTable dtb = _dbOperations.ExecuteTable($"select * from ref_user_role where user_id={id} and is_active = '2100-12-31'", null);
            if (dtb.Rows.Count != 0)
            {

                if (dtb.Rows[0]["role_id"].ToString() != roleId)
                {
                    query = $"update ref_user_role set is_active = '{UtilService.DoMSql(DateOnly.FromDateTime(DateTime.Now))}' where user_id='{id}' and is_active = '2100-12-31'";
                    //query = $"delete from ref_user_role where user_id='{userid}'and is_active = '2100-12-31'";
                    _dbOperations.ExecuteNonTransaction((cmd) => {

                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            throw new DataException("Unable to update role for user");
                        }
                    }, query);

                    query = $"insert into ref_user_role (user_id,role_id,created_date)values(@user_id,@role_id,@created_date)";
                    _dbOperations.ExecuteNonTransaction((cmd) =>
                    {
                        cmd.Parameters.AddWithValue("user_id", id);
                        cmd.Parameters.AddWithValue("role_id", roleId);
                        cmd.Parameters.AddWithValue("created_date", UtilService.DtMSql(DateTime.Now));
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            throw new DataException("Unable to add user_role reference");
                        }
                    }, query);



                }
            }


        }
    }
}


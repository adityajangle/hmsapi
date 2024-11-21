using System;
using System.Collections.Immutable;
using System.Data;
using hmsapi.Data;

namespace hmsapi.Data
{
    public class col_mst_permission
    {
        public string id { get; set; } = null!;
        public string name { get; set; } = null!;
        public string category { get; set; } = null!;
        public string? description { get; set; }
        public DateTime wef_date { get; set; }
        public bool status { get; set; }
        public string? url { get; set; }
    }

    public class mst_Permission
    {
        public static readonly ImmutableList<col_mst_permission> AllPermission = ImmutableList.Create(
            new col_mst_permission()
            {
                description = "",
                id = "PRM_DASHVIEW",
                name = "Dashboard View",
                category = "Dashboard",
                status = true,
                wef_date = DateTime.Now,
                url = "",
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_ADASHVIEW",
                name = "Admin Dashboard View",
                category = "Dashboard",
                status = true,
                wef_date = DateTime.Now
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_DEPTUPDATE",
                category = "Department",
                name = "Department Update",
                status = true,
                wef_date = DateTime.Now,
                url = "/Department/UpdateDepartment"
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_DEPTCREATE",
                category = "Department",
                name = "Department CREATE",
                status = true,
                wef_date = DateTime.Now,
                url = "/Department/CreateDepartment"
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_USERUPDATE",
                name = "User Update",
                category = "User",
                status = true,
                wef_date = DateTime.Now,
                url = "/User/UpdateUser"
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_USERCREATE",
                name = "User CREATE",
                category = "User",
                status = true,
                wef_date = DateTime.Now,
                url = "/User/CreateUser"
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_DESGCREATE",
                name = "Designation CREATE",
                category = "Designation",
                status = true,
                wef_date = DateTime.Now,
                url = "/Designation/CreateDesignation"
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_ROLEUPDATE",
                name = "Role Update",
                category = "Role",
                status = true,
                wef_date = DateTime.Now,
                url = "/Roles/UpdateRole"
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_ROLECREATE",
                name = "Role CREATE",
                category = "Role",
                status = true,
                wef_date = DateTime.Now,
                url = "/Roles/CreateRole"
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_ACCESSMST",
                name = "Master view Access",
                category = "Access",
                status = true,
                wef_date = DateTime.Now
            },
            new col_mst_permission()
            {
                description = "",
                id = "PRM_ACCESSRPT",
                name = "Report view Access",
                category = "Access",
                status = true,
                wef_date = DateTime.Now
            }
            );



        public static void BulkInsertTblPermission(IDbOperations _dbOperations)
        {
            AllPermission.ForEach(col_mst_permission =>
            {
                List<string> _columns = new List<string>();
                col_mst_permission.GetType().GetProperties().ToList().ForEach(
                x => _columns.Add(x.Name));

                string query = $"INSERT INTO mst_permission({string.Join(',', _columns.ToArray())})VALUES( {string.Join(',', _columns.Select(x => $"@{x}").ToArray())})";
                _dbOperations.ExecuteNonTransaction((command) => {
                    _dbOperations.AddCommandParams(col_mst_permission, command);

                    int result = command.ExecuteNonQuery();
                    if (result == 0)
                    {
                        throw new Exception("Unable add permissions");
                    }

                }, query);

            });
        }

        public static List<col_mst_permission?> GetAllPermissions(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_permission", null);
            List<col_mst_permission?> permissionList = new List<col_mst_permission?>();
            foreach (DataRow x in dtb.Rows)
            {
                col_mst_permission? permission = new col_mst_permission();
                permission = _dbOperations.SelectParsedData(permission, x);
                permissionList.Add(permission);
            }
            return permissionList;
        }




    }
}


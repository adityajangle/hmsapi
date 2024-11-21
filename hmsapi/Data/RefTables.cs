using System;
namespace hmsapi.Data
{
	public class ref_user_role {
		public string? user_id { get; set; }
		public string? role_id { get; set; }
		public DateTime created_date { get; set; }
		public DateTime is_active { get; set; }
	}



	public class ref_role_permission {
		public string? role_id { get; set; }
		public string permission_id { get; set; } = null!;
		public DateTime created_date { get; set; }
		public DateOnly is_active { get; set; }
	}

}


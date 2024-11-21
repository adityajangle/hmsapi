using System;
namespace hmsapi.Models
{
	public class DaoLogin
	{
        public string? employee_code { get; set; }
        public string? pwd { get; set; }
        public string? captch { get; set; }
    }

    public class DaoPasswordChange
    {
        public string? employee_code { get; set; }
        public string? old_pwd { get; set; }
        public string? new_pwd { get; set; }
    }
}


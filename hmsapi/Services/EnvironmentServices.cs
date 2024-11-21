using System;
namespace hmsapi.Services
{
    public static class  EnvironmentServices
	{
		 
            public static bool IsDevelopment()
            {
                string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            return string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase);
            }

            public static bool IsStaging()
            {
                string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
                return string.Equals(environment, "Staging", StringComparison.OrdinalIgnoreCase);
            }

            public static bool IsProduction()
            {
                string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            return false;//string.Equals(environment, "Production", StringComparison.OrdinalIgnoreCase);
            }
        
	}
}


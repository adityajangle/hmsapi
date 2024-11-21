using System;
using hmsapi.Models;
using Serilog;
using Serilog.Events;

namespace hmsapi.Services

{
	public static class JLogger
	{
        public static void WRT(DaoJLogger daoJLogger)
        {

            string p_fldr_name = "CMS_LOG";

            DateTime p_dt = DateTime.Now.Date;

            string p_dir = $"C:\\Temp\\{p_fldr_name}\\{p_dt:yyMM}";
            if (EnvironmentServices.IsDevelopment())
            {
                p_dir = $"/Users/adityajangle/Projects/temp/{p_fldr_name}/{p_dt:yyMM}";

            }

            if (!File.Exists(p_dir))
                Directory.CreateDirectory(p_dir);


            p_dir = $"C:\\Temp\\{p_fldr_name}\\{p_dt:yyMM}\\{p_dt:yyMMdd}";
            if (EnvironmentServices.IsDevelopment())
            {
                p_dir = $"/Users/adityajangle/Projects/temp/{p_fldr_name}/{p_dt:yyMM}/{p_dt:yyMMdd}";
            }

            if (!File.Exists(p_dir))
                Directory.CreateDirectory(p_dir);

            string p_log = p_dir + @"/LOG_" + daoJLogger.FileName + ".txt";

            var log = new LoggerConfiguration()
                .WriteTo.File(p_log, restrictedToMinimumLevel: LogEventLevel.Information)
                            .CreateLogger();

            log.Write(daoJLogger.LogEventLevel, daoJLogger.Message);
            log.Dispose();

        }
    }
}


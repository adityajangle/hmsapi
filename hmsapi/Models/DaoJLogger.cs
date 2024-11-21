using System;
using Serilog.Events;

namespace hmsapi.Models
{
	public class DaoJLogger
	{
        public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Information;
        public string Message { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }

    
}


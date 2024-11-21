using System;
namespace hmsapi.Models
{
    
    public class DaoResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = "";
        public object? Payload { get; set; }
    }
    public class DaoRequest
    {
        public string Message { get; set; } = null!;
        public string Payload { get; set; } = null!;
    }
}


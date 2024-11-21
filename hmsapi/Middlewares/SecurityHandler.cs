using System;
using System.Net;
using System.Security.Policy;
using hmsapi.Managers;
using hmsapi.Models;
using hmsapi.Services;
using Newtonsoft.Json;

namespace hmsapi.Middlewares
{
    public class SecurityHandler
    {
        private readonly SessionManager _sessionManager;
        private readonly RequestDelegate _next;
        public SecurityHandler(RequestDelegate next, SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                string? hash = context.Request.Headers[JSecConst.vcap_request];
                if (context.Request.Method != "POST")
                {
                    throw new UnauthorizedAccessException("Invalid Request Method");
                }
                if(hash == null && EnvironmentServices.IsProduction()) {
                    throw new UnauthorizedAccessException("Invalid hash");
                }
                if ((_sessionManager.AesKey ==null || _sessionManager.AesIv == null) && EnvironmentServices.IsProduction())
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    _ = context.Response.WriteAsync("Invalid Session or Session Expired..!");
                    return;
                }


                string[] path = context.Request.Path.ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);
                if (path.Where(x => x == "Auth").Any())
                {
                    Stream stream = context.Request.Body;
                    using (StreamReader data = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        string d = await data.ReadToEndAsync();
                        JLogger.WRT(new DaoJLogger()
                        {
                            FileName = $"HSK_{context.Session.Id}",
                            Message = $"Request:{context.Request.Path} \nRequest:{d} \n"
                        });
                        DaoRequest clientRequest = JsonConvert.DeserializeObject<DaoRequest>(d)!;
                        //string _result = JSecurityServices.ComputeSHA256(clientRequest.Message);
                        //if (hash != _result && EnvironmentServices.IsProduction())
                        //{
                        //    throw new UnauthorizedAccessException("invalid hash");
                        //}
                        _sessionManager.RequestData = clientRequest;
                    }
                }
                else
                {
                    Stream stream = context.Request.Body;
                    using (StreamReader data = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        string readData = await data.ReadToEndAsync();
                        //string enData = JsonConvert.DeserializeObject<string>(readData)!;
                        string plData = EnvironmentServices.IsProduction() ?  JSecurityServices.AES_Decrypt(readData, _sessionManager.AesKey, _sessionManager.AesIv): readData;
                        string _result = JSecurityServices.ComputeSHA256(plData);
                        if (hash != _result && EnvironmentServices.IsProduction())
                        {
                            throw new UnauthorizedAccessException("invalid hash");
                        }
                        DaoRequest d = JsonConvert.DeserializeObject<DaoRequest>(plData)!;
                        _sessionManager.RequestData = d;
                        JLogger.WRT(new DaoJLogger()
                        {
                            FileName = $"REQ_{context.Session.Id}",
                            Message = $"URL: {context.Request.Path} \nRequest START \nRequest: {plData.Replace("/", "")}"
                        });
                    }
                }
                await _next(context);

            }
            catch (UnauthorizedAccessException ex)
            {
                object ip = context.Connection.RemoteIpAddress;
                if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    ip = context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0];
                }
                JLogger.WRT(new DaoJLogger()
                {
                    FileName = $"UNAUTH{context.Session.Id}",
                    Message = $"Unauthorised: {ex} ip:{ip} port:{context.Connection.RemotePort}"
                });
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                _ = context.Response.WriteAsync("Contact Administrator - Not allowed");
            }
        }
    }
}


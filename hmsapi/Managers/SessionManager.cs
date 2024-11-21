using hmsapi.Data;
using hmsapi.Models;
using Newtonsoft.Json;

namespace hmsapi.Managers
{
	public class SessionManager
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public SessionManager(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
		{
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            IDbOperations iDboperation = new DbOperations(configuration);
            DbStructure.Init(iDboperation);

        }
        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public DaoRequest RequestData
        {
            get => GetItem();
            set => SetItem(value);
        }

        public byte[]? AesKey
        {
            get => Get<byte[]>("AesKey");
            set => Set("AesKey", value);
        }

        public byte[]? AesIv
        {
            get => Get<byte[]>("AesIv");
            set => Set("AesIv", value);
        }
        public string? AuthCaptcha
        {
            get => Get<string>("AuthCaptcha");
            set => Set("AuthCaptcha", value);
        }

        private T? Get<T>(string key)
        {
            var value = Session.GetString(key);
            return value != null ? JsonConvert.DeserializeObject<T>(value) : default;
        }

        private void Set<T>(string key, T value)
        {
            Session.SetString(key, JsonConvert.SerializeObject(value));
        }


        private DaoRequest GetItem()
        {
            if (_httpContextAccessor.HttpContext!.Items["Payload"] == null)
            {
                throw new KeyNotFoundException("Item Payload Missing");
            }
            string _r = (string)_httpContextAccessor.HttpContext!.Items["Payload"]!;
            var _s = JsonConvert.DeserializeObject<DaoRequest>(_r)!;
            return _s;
        }

        private void SetItem(DaoRequest request)
        {
            var _r = JsonConvert.SerializeObject(request);
            _httpContextAccessor.HttpContext!.Items.Add("Payload", _r);
        }

    }
}


using System.Net;

namespace HubTelWalletApi.Models
{
  
        public abstract class BaseResponseData
        {
        public string Message { get; set; } = string.Empty;
            public HttpStatusCode StatusCode { get; set; }
        }
    
}

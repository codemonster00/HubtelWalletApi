using System.Net;
using System.Text.Json.Serialization;

namespace HubTelWalletApi.Models
{
    public class SignUpResponse: BaseResponseData
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Otp { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RequestId { get; set; } = string.Empty;
        public string Phone { get;  set; }
    }

   
       
      
     
    
}

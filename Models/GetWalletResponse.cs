using System.Text.Json.Serialization;

namespace HubTelWalletApi.Models
{
    public class GetWalletResponse: BaseResponseData
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Wallet? Wallet { get; set; }
    }
}

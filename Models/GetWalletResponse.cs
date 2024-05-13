using System.Text.Json.Serialization;

namespace HubTelWalletApi.Models
{
    public class GetWalletResponse: BaseResponseData
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WalletDTO? Wallet { get; set; }
    }
}

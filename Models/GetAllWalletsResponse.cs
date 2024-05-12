namespace HubTelWalletApi.Models
{
    public class GetAllWalletsResponse: BaseResponseData
    {
        public List<Wallet>? Wallets{ get; set; }

        public void Add(Wallet wallet)
        {
            Wallets?.Add(wallet);
        }
    }

    
}

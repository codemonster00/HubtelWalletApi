using AutoMapper;
namespace HubTelWalletApi.Models

{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Wallet, WalletDTO>();
            CreateMap<WalletDTO, Wallet>();
        }
    }
}

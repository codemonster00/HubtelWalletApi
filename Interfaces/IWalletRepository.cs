using HubTelWalletApi.Models;

namespace HubTelWalletApi.Interfaces
{
    public interface IWalletRepository
    {

        Task<bool> AddAsync(Wallet wallet);
       Task<List<Wallet>> GetAllAsync(string phone);

       Task<Wallet?> GetAsync(int id,string phone);

        public Task<bool> DeleteAsync(int id,string phone);
        public string GetMobilePhoneClaimFromJwt(string? jwtToken);

        public Task<bool> WalletExist(Wallet wallet);
        public  Task<bool> ExceedCreateWalletLimit(Wallet wallet);

        public Task<AppUser> GetAppUserFromPhone(string phone);


    }
}

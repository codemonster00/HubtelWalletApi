using HubTelWalletApi.Models;

namespace HubTelWalletApi.Interfaces
{
    public interface IUserRepository
    {

        void SaveUser(AppUser User);
        bool UserExist(string Phone);
    }
}

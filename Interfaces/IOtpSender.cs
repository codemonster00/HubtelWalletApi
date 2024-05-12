using HubTelWalletApi.Models;

namespace HubTelWalletApi.Interfaces
{
    public interface IOtpSender
    {
        OtpData SendOtp(string usephone);

        bool Verify(OtpData data);
    }
}

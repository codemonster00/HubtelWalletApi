using HubTelWalletApi.Interfaces;
using HubTelWalletApi.Models;

namespace HubTelWalletApi.Services
{
    public class UssdOtpSender : IOtpSender
    {
        public OtpData SendOtp(string userphone)
        {
            return new OtpData{ Otp="1234",RequestId="1234",Phone= userphone };
        }

        public bool Verify(OtpData data)
        {
            return true;
        }
    }
}

using HubTelWalletApi.Context;
using HubTelWalletApi.Interfaces;
using HubTelWalletApi.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HubTelWalletApi.Services
{
    public class UssdOtpSender : IOtpSender
    {

        private readonly AppDbContext _context;
        //Persists otp for verfication for lack of good ussd
        //implementation 

        public UssdOtpSender(AppDbContext context)
        {
                _context = context;
        }
        public OtpData SendOtp(string userphone)
        {
            string otp = new Random().Next(1000, 9999).ToString();
            var requestid = Guid.NewGuid().ToString();

            var otpdata = new OtpData { Otp = otp, RequestId = requestid, Phone = userphone };
            _context.OtpData.Add(otpdata);
            _context.SaveChanges();
            return otpdata ;
        }

        public bool Verify(OtpData data)
        {
            if (data is null) { 
                return false;
            }
            return _context.OtpData.Any(x => x.RequestId == data.RequestId &&x.Otp==data.Otp);
            
        }
    }
}

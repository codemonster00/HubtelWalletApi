using HubTelWalletApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestHubtelWalletApi.MockData
{
    public  class WalletMockData
    {
        public static List<Wallet> GetWallets()
        {
            return new List<Wallet>
            {
                new Wallet{Id=1,Name="First",OwnerId=5,Type= WalletType.Momo ,AccountNumber="405394939",Scheme=AccountScheme.Mtn},
                new Wallet{Id=2,Name="Second",OwnerId=5,Type= WalletType.Card ,AccountNumber="1122394939",Scheme=AccountScheme.Visa},
                new Wallet{Id=3,Name="Third",OwnerId=5,Type= WalletType.Momo ,AccountNumber="5678394939",Scheme=AccountScheme.Vodafone },
                new Wallet{Id=4,Name="Fourth",OwnerId=5,Type= WalletType.Card ,AccountNumber="5040694939",Scheme=AccountScheme.Mastercard },
                new Wallet{Id=5,Name="Fifth",OwnerId=5,Type= WalletType.Momo ,AccountNumber="890394939",Scheme=AccountScheme.AirtelTigo },
            };
        }
    }
}

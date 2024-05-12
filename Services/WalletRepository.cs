using HubTelWalletApi.Context;
using HubTelWalletApi.Interfaces;
using HubTelWalletApi.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Numerics;
using System.Security.Claims;

namespace HubTelWalletApi.Services
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WalletRepository> _logger;
        public WalletRepository(AppDbContext context,ILogger<WalletRepository> logger) { 
           _logger = logger;
           _context = context;
        }

        //This method adds a new wallet asynchro......
        public async Task<bool> AddAsync(Wallet wallet)
        {
            if (wallet.Type == WalletType.Card)
            {
                wallet.AccountNumber = wallet.AccountNumber[..6];
            }
            
           _context.Wallets.Add(wallet);
            var result = await _context.SaveChangesAsync();
            _logger.LogInformation("Wallet successfully added");
            return result > 0;
            
         
        }


        //This method checks if the wallet been added already exists 
        public async Task<bool> WalletExist(Wallet wallet)
        {
            if(wallet.Type == WalletType.Card)
            {
                wallet.AccountNumber = wallet.AccountNumber[..6];
            }
            var existingwallet = await _context.Wallets.FirstOrDefaultAsync(x => x.AccountNumber == wallet.AccountNumber);
            if (existingwallet != null)
            {

                throw new InvalidOperationException("This wallet already wxisits");
            }
            return true;
        }

        //This wallet checks if user has exceeded the wallet creation limits 
        public async Task<bool> ExceedCreateWalletLimit(Wallet wallet)
        {
           var userwalletcount = await _context.Wallets.CountAsync(x=>x.Owner.Phone == wallet.Owner.Phone);
            if ( userwalletcount>=5 )
            {
                throw new InvalidOperationException("User can only create up to five wallets");
                
            }
            return true;
        }

        //This method deletes soft deletes a wallet with an id 
        public async Task<bool> DeleteAsync(int id,string phone)
        {
            try
            {
                //delete wallet with id belonging to authenticated user
                var wallettodelete = await _context.Wallets.Include(w => w.Owner).FirstOrDefaultAsync(x => x.Id == id && x.Owner.Phone == phone);
                if (wallettodelete != null)
                {
                    wallettodelete.SoftDelete();
                    var result = await _context.SaveChangesAsync();
                    return true;
                }
                throw new KeyNotFoundException("Resource not found with specified id");
            }
            catch (Exception)
            {

                throw;
            }
        

           
        }

        //This method returns  all wallet related to a particular user
        
        public async Task<List<Wallet>> GetAllAsync(string phone)
        {
            return await _context.Wallets.Where(x => x.Owner.Phone == phone).ToListAsync();
        }



       //This method returns a wallet with an id that belongs to the authenticated user
        public async Task<Wallet?> GetAsync(int id,string phone) {

            try
            {
                var wallet = await _context.Wallets.Include(w => w.Owner).FirstOrDefaultAsync(x => x.Id == id && x.Owner.Phone==phone);   // add phone in query
                if (wallet == null)
                {
                    throw new KeyNotFoundException("Wallet was not found with the specified id");
                }
                return wallet;
            }


            catch (Exception)
            {
                throw;
                
               
            }

            
        }
        public string GetMobilePhoneClaimFromJwt(string? jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwtToken);

            // Find the MobilePhone claim
            var mobilePhoneClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone);

            if (mobilePhoneClaim != null)
            {
                return mobilePhoneClaim.Value;
            }

            // Return null or handle the case when the claim is not found
            return "";
        }

        public   async Task<AppUser>  GetAppUserFromPhone(string phone)
        {
            return  await _context.Users.FirstAsync(x=>x.Phone == phone);
        }
    }
}

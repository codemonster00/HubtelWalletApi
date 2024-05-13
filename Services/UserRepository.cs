using HubTelWalletApi.Context;
using HubTelWalletApi.Interfaces;
using HubTelWalletApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HubTelWalletApi.Services
{
 
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public  void SaveUser(AppUser User)
        {
             _context.Users.Add(User);   
             _context.SaveChanges();
        }

        public  bool UserExist(string phone)
        {
           var result =   _context.Users.Any(x=>x.Phone==phone);
           
            return result;

        }
    }
}

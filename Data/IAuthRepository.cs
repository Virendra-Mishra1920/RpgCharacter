using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgCharacter.Data
{
    public interface IAuthRepository
    {
        public Task<ServiceResponse<int>> Register(User user, string password);
        public Task<ServiceResponse<string>> Login(string userName, string password);
        public Task<bool> UserAlreadyExist(string userName);
        
        
    }
}
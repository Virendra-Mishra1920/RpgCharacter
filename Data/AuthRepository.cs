using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace RpgCharacter.Data
{
    public class AuthRepository: IAuthRepository
    {
        #region  Private Properties
            private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        #endregion
        public AuthRepository(DataContext context, IConfiguration configuration )
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            ServiceResponse<int> response=new ServiceResponse<int>();

            if(await UserAlreadyExist(user.Name))
            {
                response.Success=false;
                response.Message=$"User already exist with userName {user.Name}";
                return response;
            }

            CreateHashPassword(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            response.Data=user.Id;
            return response;



        }

        public async Task<ServiceResponse<string>> Login(string userName, string password)
        {
            var response =new ServiceResponse<string>();
            var user= await _context.Users.FirstOrDefaultAsync(t=>t.Name.ToLower().Equals(userName.ToLower()));

            if(user==null)
            {
                response.Success=false;
                response.Message=$"User not found with userName {userName}";

            }

            else if(!await VerifyHashPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success=false;
                response.Message="Wrong password";

            }

            else
            {
                response.Data= await CreateJwtToken(user);
            }

            return response;

        }

        

        #region  private methods

        public void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt=hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        public async Task<bool> UserAlreadyExist(string userName)
        {
            if(await _context.Users.AnyAsync(t=>t.Name.ToLower()== userName.ToLower()))
            {
                return true;
            }

            return false;
        }

        private async Task<bool> VerifyHashPassword(string password, byte[]? passwordHash, byte[]? passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }

        }

        private async Task<string> CreateJwtToken(User user)
        {
            // create claims
            List<Claim> claims=new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)

            };

            // create symmetric security key
           SymmetricSecurityKey key=new SymmetricSecurityKey(System.Text.Encoding.UTF8.
           GetBytes(_configuration.GetSection("AppSettings:Token").Value));

           // signing credentials 
           SigningCredentials credentials=new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // create security token descriptor
           SecurityTokenDescriptor descriptor=new SecurityTokenDescriptor
           {
               Subject=new ClaimsIdentity(claims),
               Expires=System.DateTime.Now.AddDays(1),
               SigningCredentials=credentials
           };

           // create token
           JwtSecurityTokenHandler handler=new JwtSecurityTokenHandler();
           SecurityToken token=handler.CreateToken(descriptor);

           return handler.WriteToken(token);
        }

        #endregion


        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgCharacter.Dtos.Character
{
    public class UserRegisterDto
    {
        public string userName { get; set; }=string.Empty;
        public string Password { get; set; }=string.Empty;
        
    }
}
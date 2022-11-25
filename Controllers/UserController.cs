using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RpgCharacter.Data;
using RpgCharacter.Dtos.Character;

namespace RpgCharacter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController:ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public UserController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("Register")]
        public async  Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
        {
           var response=await _authRepository.Register(new User{Name=request.userName}, request.Password);

           if(!response.Success)
           {
               return BadRequest(response);
           }

           return Ok(response);
        }

         [HttpPost("Login")]
        public async  Task<ActionResult<ServiceResponse<int>>> Register(UserLoginDto request)
        {
           var response=await _authRepository.Login(request.userName, request.Password);

           if(!response.Success)
           {
               return BadRequest(response);
           }

           return Ok(response);
        }
        
    }
}
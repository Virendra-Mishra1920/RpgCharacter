using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RpgCharacter.Dtos.Character;
using RpgCharacter.Services.CharacterService;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RpgCharacter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController:ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

       
        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse <List<GetCharacterDto>>>> GetAll()
        {
            // read claims 
            int userId=int.Parse(User.Claims.FirstOrDefault(u=>u.Type== ClaimTypes.NameIdentifier).Value);
            return Ok(await _characterService.GetAllCharacter(userId));
        }

         [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse< GetCharacterDto>>> GetSingle(int id)
        {

            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost("AddCharacter")]
        public async Task< ActionResult<ServiceResponse<GetCharacterDto>>> AddCharacter(AddCharacterDto character)
        {
            
            return Ok( await _characterService.AddCharacter(character));
        }

         [HttpPut("UpdateCharacter")]
        public async Task< ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto updateCharacterDto)
        {
            var response=await _characterService.UpdateCharacter(updateCharacterDto);
            if(response.Data==null)
            return NotFound(response);
            
            return Ok( response);
        }


         [HttpDelete("{id}/DeleteCharacter")]
        public async Task< ActionResult<ServiceResponse<List< GetCharacterDto>>>> DeleteCharacter( int id)
        {
            var response=await _characterService.DeleteCharacter(id);
            if(response.Data==null)
            return NotFound(response);
            
            return Ok( response);
        }
        
    }
}
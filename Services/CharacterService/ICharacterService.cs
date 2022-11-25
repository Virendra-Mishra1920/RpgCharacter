using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgCharacter.Dtos.Character;

namespace RpgCharacter.Services.CharacterService
{
    public interface ICharacterService
    {
        public Task<ServiceResponse< List<GetCharacterDto>>> GetAllCharacter(int userId);
        public Task<ServiceResponse< GetCharacterDto>> GetCharacterById(int id);
        public Task<ServiceResponse< List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);
        public Task< ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacterDto);

        public Task<ServiceResponse<List< GetCharacterDto>>> DeleteCharacter(int id);
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RpgCharacter.Dtos.Character;
using RpgCharacter.Data;
using Microsoft.EntityFrameworkCore;

namespace RpgCharacter.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
       {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var response=new ServiceResponse<List<GetCharacterDto>>(); 
            Character character=_mapper.Map<Character>(newCharacter);
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

             response.Data= _context.Characters.Select(c=>_mapper.Map<GetCharacterDto>(c)).ToList();
             return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
             var response=new ServiceResponse<List<GetCharacterDto>>();

             try
             {
                Character character=await _context.Characters.FirstOrDefaultAsync(c=>c.Id==id);
                if(character==null)
                {
                    response.Success=false;
                    response.Message=$"Character not found with id {id}";
                    return response;
                }
            
               _context.Characters.Remove(character);
               await _context.SaveChangesAsync();
               response.Data= _context.Characters.Select(c=>_mapper.Map<GetCharacterDto>(c)).ToList();  
             }
             catch (System.Exception ex)
             {
                 response.Success=false;
                 response.Message=ex.Message;
                
             }

             return response;

            
            
        }

        public async Task<ServiceResponse< List<GetCharacterDto>>> GetAllCharacter(int userId)
        {
            var response=new ServiceResponse<List<GetCharacterDto>>();
            var dbChars= await _context.Characters.
            Where(u=>u.User.Id==userId).ToListAsync();
            response.Data=dbChars.Select(c=>_mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var response=new ServiceResponse<GetCharacterDto>();
            var res= await _context.Characters.FirstOrDefaultAsync(t=>t.Id==id);
            if(res==null)
            {
                response.Success=false;
                response.Message=$"Cant found the Id {id}";
                return response;

            }
           
            response.Data=_mapper.Map<GetCharacterDto>(res) ;
            response.Message=$"Record successfully fetched with Id {id}";
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacterDto)
        {
            var response=new ServiceResponse<GetCharacterDto>();
            try
            {
                
                Character character=await _context.Characters.FirstOrDefaultAsync(c=>c.Id==updateCharacterDto.Id);
                if(character==null)
                {
                    response.Success=false;
                    response.Message=$"Character not found with Id {updateCharacterDto.Id}";
                    return response;
                }

                //_mapper.Map(updateCharacterDto,character);

                character.Name=updateCharacterDto.Name;
                character.HitPoints=updateCharacterDto.HitPoints;
                character.Class=updateCharacterDto.Class;
                character.Defence=updateCharacterDto.Defence;
                character.Intelligence=updateCharacterDto.Intelligence;
                character.Strength=updateCharacterDto.Strength;
                await _context.SaveChangesAsync();
                response.Data=_mapper.Map<GetCharacterDto>(character);
                

            }

            catch(Exception ex)
            {
                response.Success=false;
                response.Message=ex.Message;
            }

            return response;
        }
    }
}
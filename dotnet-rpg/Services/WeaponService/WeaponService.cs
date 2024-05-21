using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dotnet_rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private IMapper _mapper;
        private DataContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        public WeaponService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor) 
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto weaponDto)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == weaponDto.CharacterId && c.User.Id == int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
                if (character == null)
                {
                    serviceResponse.success = false;
                    serviceResponse.message = "Character not found!";
                    return serviceResponse;
                }
                Weapon weapon = new Weapon
                {
                    Name = weaponDto.Name,
                    Damage = weaponDto.Damage,
                    Character = character,
                };
                _context.Weapons.Add(weapon);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.message = ex.Message;
            }
            return serviceResponse;
        }
    }
}

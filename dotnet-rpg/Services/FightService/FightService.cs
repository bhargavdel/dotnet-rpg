using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.FightService
{
    public class FightService:IFightService
    {
        private DataContext _context;
        public FightService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters.Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == weaponAttack.AttackerId);
                var opponent = await _context.Characters.Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == weaponAttack.OpponentId);
                if (attacker == null)
                {
                    response.success = false;
                    response.message = "Attacker not found!";
                    return response;
                }
                if(opponent== null)
                {
                    response.success = false;
                    response.message = "Opponent not found!";
                    return response;
                }
                int damage = attacker.Weapon.Damage + attacker.Strength;
                damage -= opponent.Defense;
                if (damage > 0)
                {
                    opponent.HitPoints -= damage;
                }
                if (opponent.HitPoints <= 0)
                {
                    response.message = $"{opponent.Name} has been defeated";
                }
                await _context.SaveChangesAsync();
                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttack)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters.Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == skillAttack.AttackerId);
                var opponent = await _context.Characters.Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == skillAttack.OpponentId);
                if (attacker == null)
                {
                    response.success = false;
                    response.message = "Attacker not found!";
                    return response;
                }
                if (opponent == null)
                {
                    response.success = false;
                    response.message = "Opponent not found!";
                    return response;
                }
                var skill = attacker.Skills.FirstOrDefault(s => s.Id == skillAttack.SkillId);
                if(skill == null)
                {
                    response.success = false;
                    response.message = "Skill not found!";
                    return response;
                }
                int damage = skill.Damage + attacker.Strength;
                damage -= opponent.Defense;
                if (damage > 0)
                {
                    opponent.HitPoints -= damage;
                }
                if (opponent.HitPoints <= 0)
                {
                    response.message = $"{opponent.Name} has been defeated";
                }
                await _context.SaveChangesAsync();
                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch(Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
            }
            return response;
        }
    }
}

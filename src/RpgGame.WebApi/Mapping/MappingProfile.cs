using AutoMapper;
using RpgGame.Application.Commands.Characters;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Enums;
using RpgGame.WebApi.DTOs.Characters;
using System;

namespace RpgGame.WebApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Character mappings - unified Character entity
            CreateMap<Character, CharacterSummaryDto>()
                .ForMember(dest => dest.HealthPercentage,
                    opt => opt.MapFrom(src => (int)((double)src.Health / src.MaxHealth * 100)));

            CreateMap<Character, CharacterDto>()
                .ForMember(dest => dest.CharacterType, opt => opt.MapFrom(src => GetCharacterClass(src)))
                .ForMember(dest => dest.CriticalChance, opt => opt.MapFrom(src => GetCriticalChance(src)))
                .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => GetMana(src)))
                .ForMember(dest => dest.MaxMana, opt => opt.MapFrom(src => GetMaxMana(src)));

            // Command mappings
            CreateMap<CreateCharacterDto, CreateCharacterCommand>();
        }

        private string GetCharacterClass(Character character)
        {
            return character.PlayerClass?.ToString() ?? "Unknown";
        }

        private int GetCriticalChance(Character character)
        {
            // For unified Character, critical chance could be stored in custom data or calculated
            if (character.PlayerClass == PlayerClass.Rogue)
            {
                // Rogue characters have higher critical chance
                return 15; // Base critical chance for rogues
            }
            return 5; // Default critical chance
        }

        private int GetMana(Character character)
        {
            // For unified Character, mana could be stored in custom data
            if (character.PlayerClass == PlayerClass.Mage)
            {
                return character.CustomData.TryGetValue("CurrentMana", out var mana) 
                    ? Convert.ToInt32(mana) 
                    : character.Stats.Magic * 10; // Default mana calculation
            }
            return 0; // Non-mage characters don't have mana
        }

        private int GetMaxMana(Character character)
        {
            // For unified Character, max mana could be stored in custom data
            if (character.PlayerClass == PlayerClass.Mage)
            {
                return character.CustomData.TryGetValue("MaxMana", out var maxMana) 
                    ? Convert.ToInt32(maxMana) 
                    : character.Stats.Magic * 10; // Default max mana calculation
            }
            return 0; // Non-mage characters don't have mana
        }
    }
}
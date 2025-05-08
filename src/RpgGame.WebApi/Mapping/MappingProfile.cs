using AutoMapper;
using RpgGame.Application.Commands.Characters;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Characters.Player;
using RpgGame.Domain.Enums;
using RpgGame.WebApi.DTOs.Characters;
using System;

namespace RpgGame.WebApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Character mappings
            CreateMap<Character, CharacterSummaryDto>()
                .ForMember(dest => dest.HealthPercentage,
                    opt => opt.MapFrom(src => (int)((double)src.Health / src.MaxHealth * 100)));

            CreateMap<PlayerCharacter, CharacterDto>()
                .Include<Warrior, CharacterDto>()
                .Include<Mage, CharacterDto>()
                .Include<Rogue, CharacterDto>()
                .ForMember(dest => dest.CharacterType, opt => opt.MapFrom(src => GetCharacterType(src)))
                .ForMember(dest => dest.CriticalChance, opt => opt.Ignore())
                .ForMember(dest => dest.Mana, opt => opt.Ignore())
                .ForMember(dest => dest.MaxMana, opt => opt.Ignore());

            CreateMap<Warrior, CharacterDto>();

            CreateMap<Mage, CharacterDto>()
                .ForMember(dest => dest.Mana, opt => opt.MapFrom(src => src.Mana))
                .ForMember(dest => dest.MaxMana, opt => opt.MapFrom(src => src.MaxMana));

            CreateMap<Rogue, CharacterDto>()
                .ForMember(dest => dest.CriticalChance, opt => opt.MapFrom(src => src.CriticalChance));

            // Command mappings
            CreateMap<CreateCharacterDto, CreateCharacterCommand>();
        }

        private CharacterType GetCharacterType(PlayerCharacter character)
        {
            return character switch
            {
                Warrior => CharacterType.Warrior,
                Mage => CharacterType.Mage,
                Rogue => CharacterType.Rogue,
                _ => throw new ArgumentException($"Unknown character type: {character.GetType().Name}")
            };
        }
    }
}
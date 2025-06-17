﻿// Rusing RpgGame.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using RpgGame.Domain.Enums;

namespace RpgGame.WebApi.DTOs.Characters
{
    public class CreateCharacterDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public CharacterType Type { get; set; }
    }
}
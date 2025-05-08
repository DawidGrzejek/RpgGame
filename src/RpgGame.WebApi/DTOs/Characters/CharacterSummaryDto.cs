using RpgGame.Domain.Enums;
using System;

namespace RpgGame.WebApi.DTOs.Characters
{
    public class CharacterSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public CharacterType CharacterType { get; set; }
        public int HealthPercentage { get; set; }
    }
}
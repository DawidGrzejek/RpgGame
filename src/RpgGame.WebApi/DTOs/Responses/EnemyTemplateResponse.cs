using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.WebApi.DTOs.Responses
{
    public class EnemyTemplateResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BaseHealth { get; set; }
        public int BaseStrength { get; set; }
        public int BaseDefense { get; set; }
        public int ExperienceReward { get; set; }
        public string EnemyType { get; set; }
        public List<string> PossibleLoot { get; set; }
        public Dictionary<string, object> SpecialAbilities { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
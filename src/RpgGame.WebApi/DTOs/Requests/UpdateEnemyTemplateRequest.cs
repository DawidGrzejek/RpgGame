using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.WebApi.DTOs.Requests
{

    public class UpdateEnemyTemplateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BaseHealth { get; set; }
        public int BaseStrength { get; set; }
        public int BaseDefense { get; set; }
        public int ExperienceReward { get; set; }
        public string EnemyType { get; set; }
        public List<string> PossibleLoot { get; set; } = new();
        public Dictionary<string, object> SpecialAbilities { get; set; } = new();
    }
}
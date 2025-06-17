using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Results
{
    public class CombatResult
    {
        public int PlayerDamage { get; set; }
        public int EnemyDamage { get; set; }
        public int PlayerHealth { get; set; }
        public int EnemyHealth { get; set; }
        public bool EnemyDefeated { get; set; }
        public bool PlayerDefeated { get; set; }
        public int ExperienceGained { get; set; }
        public List<string> ItemsDropped { get; set; } = new();
    }
}

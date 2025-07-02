using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Characters.NPC.Enemy;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Factories
{
    public class EnemyFactory
    {
        public static Enemy CreateFromTemplate(EnemyTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template), "Enemy template cannot be null.");
            }

            return template.EnemyType switch
            {
                EnemyType.Beast => new Enemy(template),      // Wolf, Bear, Spider.
                EnemyType.Undead => new Enemy(template),    // Skeleton, Zombie, etc.
                EnemyType.Dragon => new Enemy(template),    // All dragons
                EnemyType.Humanoid => new Enemy(template), // Goblin, Orc, Troll, etc.
                _ => throw new ArgumentException($"Unknown enemy type: {template.EnemyType}", nameof(template))
            };
        }
    }
}
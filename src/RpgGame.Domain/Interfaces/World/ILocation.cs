using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Characters.NPC.Enemy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Interfaces.World
{
    /// <summary>
    /// Defines a location in the game world
    /// </summary>
    public interface ILocation
    {
        string Name { get; }
        string Description { get; }
        IReadOnlyList<Enemy> PossibleEnemies { get; }

        void AddPossibleEnemy(Enemy enemy);
        Enemy GetRandomEnemy();
    }
}

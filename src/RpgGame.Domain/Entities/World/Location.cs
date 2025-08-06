using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Entities.World
{
    /// <summary>
    /// Represents a location in the game world
    /// </summary>
    public class Location : ILocation
    {
        public string Name { get; }
        public string Description { get; }
        public IReadOnlyList<Character> PossibleEnemies => _possibleEnemies.AsReadOnly();
        private readonly List<Character> _possibleEnemies;

        public Location(string name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            _possibleEnemies = new List<Character>();
        }

        /// <summary>
        /// Adds an enemy type that can be encountered in this location
        /// </summary>
        public void AddPossibleEnemy(Character enemy)
        {
            if (enemy != null)
            {
                _possibleEnemies.Add(enemy);
            }
        }

        /// <summary>
        /// Returns a random enemy that can be encountered in this location
        /// </summary>
        public Character GetRandomEnemy()
        {
            if (_possibleEnemies.Count == 0)
            {
                return null;
            }

            Random rnd = new Random();
            int index = rnd.Next(_possibleEnemies.Count);

            // Clone the enemy to get a fresh instance
            // In a real implementation, we'd have a factory to create new instances
            return _possibleEnemies[index];
        }
    }
}

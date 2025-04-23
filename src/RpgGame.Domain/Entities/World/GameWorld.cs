using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Entities.World
{
    /// <summary>
    /// Represents the game world with all its locations
    /// </summary>
    public class GameWorld : IGameWorld
    {
        private Dictionary<string, Location> _locations;
        private Dictionary<string, List<string>> _connections;

        public ILocation StartLocation { get; private set; }

        public GameWorld()
        {
            // Initialize collections
            _locations = new Dictionary<string, Location>();
            _connections = new Dictionary<string, List<string>>();

            // Create locations
            CreateLocations();

            // Set starting location
            StartLocation = _locations["Town"];
        }

        /// <summary>
        /// Creates all locations in the game world
        /// </summary>
        private void CreateLocations()
        {
            // Create locations
            var town = new Location("Town", "A small peaceful town with a few shops and houses.");
            var forest = new Location("Forest", "A dense forest with tall trees and mysterious sounds.");
            var cave = new Location("Cave", "A dark cave with echoing sounds and glittering minerals.");
            var mountain = new Location("Mountain", "Rocky terrain with steep paths and a beautiful view.");

            // Add locations to dictionary
            _locations.Add("Town", town);
            _locations.Add("Forest", forest);
            _locations.Add("Cave", cave);
            _locations.Add("Mountain", mountain);

            // Add connections between locations
            AddConnection("Town", "Forest");
            AddConnection("Forest", "Cave");
            AddConnection("Forest", "Mountain");
            AddConnection("Cave", "Mountain");

            // In a real implementation, we would populate each location with
            // possible enemies, but that would require enemy implementation
        }

        /// <summary>
        /// Adds a two-way connection between locations
        /// </summary>
        private void AddConnection(string location1, string location2)
        {
            // Add first direction
            if (!_connections.ContainsKey(location1))
            {
                _connections[location1] = new List<string>();
            }
            if (!_connections[location1].Contains(location2))
            {
                _connections[location1].Add(location2);
            }

            // Add opposite direction
            if (!_connections.ContainsKey(location2))
            {
                _connections[location2] = new List<string>();
            }
            if (!_connections[location2].Contains(location1))
            {
                _connections[location2].Add(location1);
            }
        }

        /// <summary>
        /// Gets all locations connected to the specified location
        /// </summary>
        public List<ILocation> GetConnectedLocations(ILocation currentLocation)
        {
            var result = new List<ILocation>();

            if (currentLocation != null && _connections.ContainsKey(currentLocation.Name))
            {
                foreach (var locationName in _connections[currentLocation.Name])
                {
                    if (_locations.ContainsKey(locationName))
                    {
                        result.Add(_locations[locationName]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a location by its name
        /// </summary>
        public ILocation GetLocation(string name)
        {
            if (_locations.ContainsKey(name))
            {
                return _locations[name];
            }

            return null;
        }
    }
}

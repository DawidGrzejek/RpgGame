using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Interfaces.World
{
    /// <summary>
    /// Defines the game world with locations and connections
    /// </summary>
    public interface IGameWorld
    {
        ILocation StartLocation { get; }

        List<ILocation> GetConnectedLocations(ILocation currentLocation);
        ILocation GetLocation(string name);
    }
}

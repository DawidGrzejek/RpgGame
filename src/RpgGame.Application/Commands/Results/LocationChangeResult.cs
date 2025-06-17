using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Results
{
    public class LocationChangeResult
    {
        public ILocation NewLocation { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

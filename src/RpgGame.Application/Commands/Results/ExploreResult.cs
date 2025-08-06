using RpgGame.Domain.Entities.Characters.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Results
{
    public class ExploreResult
    {
        public bool EnemyEncountered { get; set; }
        public Character Enemy { get; set; }
        public bool ItemFound { get; set; }
        public string ItemName { get; set; }
        public int ExperienceGained { get; set; }
        public string Message { get; set; }
    }
}

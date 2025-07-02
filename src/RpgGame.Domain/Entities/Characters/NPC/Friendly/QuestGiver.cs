using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Domain.Entities.Characters.Base;

namespace RpgGame.Domain.Entities.Characters.NPC.Friendly
{
    public abstract class QuestGiver : NonPlayerCharacter
    {
        protected QuestGiver(string name, int health, int strength, int defense, bool isFriendly, string dialogue) : base(name, health, strength, defense, isFriendly, dialogue)
        {
        }
    }
}

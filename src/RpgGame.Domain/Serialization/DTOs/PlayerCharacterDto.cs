using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Serialization.DTOs
{
    [Serializable]
    public class PlayerCharacterDto
    {
        // Base Character properties
        public string CharacterType { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Level { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }

        // PlayerCharacter properties
        public int Experience { get; set; }
        public InventoryDto Inventory { get; set; }
        public Dictionary<string, object> EquippedItems { get; set; } = new Dictionary<string, object>();

        // Character type specific properties
        public double? CriticalChance { get; set; } // For Rogue
        public int? Mana { get; set; } // For Mage
        public int? MaxMana { get; set; } // For Mage
    }
}

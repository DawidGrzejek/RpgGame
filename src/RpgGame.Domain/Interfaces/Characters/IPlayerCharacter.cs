using RpgGame.Domain.Interfaces.Inventory;
using RpgGame.Domain.Interfaces.Items;

namespace RpgGame.Domain.Interfaces.Characters
{
    /// <summary>
    /// Interface for player-controlled characters
    /// </summary>
    public interface IPlayerCharacter : ICharacter
    {
        int Experience { get; }
        int ExperienceToNextLevel { get; }
        IInventory Inventory { get; }

        void GainExperience(int amount);
        void EquipItem(IEquipment item);
        void UseItem(IItem item);
        void UseSpecialAbility(ICharacter target);
    }
}

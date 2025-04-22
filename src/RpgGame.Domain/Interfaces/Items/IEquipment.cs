using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Characters;

namespace RpgGame.Domain.Interfaces.Items
{
    /// <summary>
    /// Interface for items that can be equipped
    /// </summary>
    public interface IEquipment : IItem
    {
        EquipmentSlot Slot { get; }
        int BonusValue { get; }

        void OnEquip(IPlayerCharacter character);
        void OnUnequip(IPlayerCharacter character);
    }
}

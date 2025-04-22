using RpgGame.Domain.Interfaces.Characters;

namespace RpgGame.Domain.Interfaces.Items
{
    /// <summary>
    /// Interface for items that can be consumed
    /// </summary>
    public interface IConsumable : IItem
    {
        void OnUse(IPlayerCharacter character);
    }
}

using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Interfaces.Items
{
    /// <summary>
    /// Base interface for all game items
    /// </summary>
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        int Value { get; }
        ItemType Type { get; }
    }
}

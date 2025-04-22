using System.Collections.Generic;
using RpgGame.Domain.Interfaces.Items;

namespace RpgGame.Domain.Interfaces.Inventory
{
    /// <summary>
    /// Interface for character inventories
    /// </summary>
    public interface IInventory
    {
        IReadOnlyList<IItem> Items { get; }
        int Capacity { get; }
        int Gold { get; }

        bool AddItem(IItem item);
        bool RemoveItem(IItem item);
        void AddGold(int amount);
        bool SpendGold(int amount);
    }
}

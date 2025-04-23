using System;
using System.Collections.Generic;
using System.Linq;
using RpgGame.Domain.Interfaces.Inventory;
using RpgGame.Domain.Interfaces.Items;

namespace RpgGame.Domain.Entities.Inventory
{
    /// <summary>
    /// Default implementation of the IInventory interface
    /// </summary>
    public class Inventory : IInventory
    {
        private readonly List<IItem> _items;
        private readonly int _capacity;
        private int _gold;

        /// <summary>
        /// Get the items in the inventory
        /// </summary>
        public IReadOnlyList<IItem> Items => _items.AsReadOnly();

        /// <summary>
        /// Get the maximum capacity of the inventory
        /// </summary>
        public int Capacity => _capacity;

        /// <summary>
        /// Get the amount of gold in the inventory
        /// </summary>
        public int Gold => _gold;

        /// <summary>
        /// Constructor for the inventory
        /// </summary>
        /// <param name="capacity">Maximum number of items the inventory can hold</param>
        /// <param name="initialGold">Initial amount of gold</param>
        /// <param name="initialItems">Initial items to add to the inventory</param>
        private Inventory(int capacity, int initialGold = 0, IEnumerable<IItem> initialItems = null)
        {
            _capacity = capacity;
            _gold = initialGold;
            _items = new List<IItem>();

            // Add initial items if provided
            if (initialItems != null)
            {
                foreach (var item in initialItems)
                {
                    AddItem(item);
                }
            }
        }

        /// <summary>
        /// Factory method to create a new inventory
        /// </summary>
        /// <param name="capacity">Maximum number of items the inventory can hold</param>
        /// <param name="initialGold">Initial amount of gold</param>
        /// <param name="initialItems">Initial items to add to the inventory</param>
        /// <returns>A new instance of Inventory</returns>
        public static Inventory Create(int capacity, int initialGold = 0, IEnumerable<IItem> initialItems = null)
        {
            if (capacity <= 0)
                throw new ArgumentException("Inventory capacity must be greater than zero", nameof(capacity));

            if (initialGold < 0)
                throw new ArgumentException("Initial gold amount cannot be negative", nameof(initialGold));

            return new Inventory(capacity, initialGold, initialItems);
        }


        /// <summary>
        /// Adds an item to the inventory if there is space
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>True if the item was added, false otherwise</returns>
        public bool AddItem(IItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Item cannot be null");

            if (_items.Count >= _capacity)
            {
                Console.WriteLine("Inventory is full. Cannot add item.");
                return false;
            }

            _items.Add(item);
            Console.WriteLine($"Added {item.Name} to inventory.");
            return true;
        }

        /// <summary>
        /// Removes an item from the inventory
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item was removed, false otherwise</returns>
        public bool RemoveItem(IItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Item cannot be null");

            bool removed = _items.Remove(item);

            if (removed)
            {
                Console.WriteLine($"Removed {item.Name} from inventory.");
            }
            else
            {
                Console.WriteLine($"Could not find {item.Name} in inventory.");
            }

            return removed;
        }

        /// <summary>
        /// Adds gold to the inventory
        /// </summary>
        /// <param name="amount">The amount of gold to add</param>
        public void AddGold(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Gold amount cannot be negative", nameof(amount));

            _gold += amount;
            Console.WriteLine($"Added {amount} gold. Total: {_gold}");
        }

        /// <summary>
        /// Spends gold from the inventory if enough is available
        /// </summary>
        /// <param name="amount">The amount of gold to spend</param>
        /// <returns>True if the gold was spent, false if not enough gold</returns>
        public bool SpendGold(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Gold amount cannot be negative", nameof(amount));

            if (amount > _gold)
            {
                Console.WriteLine($"Not enough gold. Required: {amount}, Available: {_gold}");
                return false;
            }

            _gold -= amount;
            Console.WriteLine($"Spent {amount} gold. Remaining: {_gold}");
            return true;
        }
    }
}
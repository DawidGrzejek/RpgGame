using System;
using System.Collections.Generic;

namespace RpgGame.WebApi.DTOs.Inventory
{
    public class InventoryDto
    {
        public Guid CharacterId { get; set; }
        public string CharacterName { get; set; }
        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
        public int Capacity { get; set; }
        public int Gold { get; set; }
        public int UsedCapacity => Items.Count;
        public int AvailableCapacity => Capacity - UsedCapacity;
    }
}
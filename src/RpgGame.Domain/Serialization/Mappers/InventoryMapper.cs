using RpgGame.Application.Serialization.DTOs;
using RpgGame.Domain.Interfaces.Inventory;
using RpgGame.Domain.Interfaces.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Serialization.Mappers
{
    public static class InventoryMapper
    {
        public static InventoryDto ToDto(IInventory inventory)
        {
            return new InventoryDto
            {
                Items = inventory.Items.Select(ItemMapper.ToDto).ToList(),
                Capacity = inventory.Capacity,
                Gold = inventory.Gold
            };
        }

        public static IInventory FromDto(InventoryDto dto)
        {
            // Convert DTOs to Items
            List<IItem> items = dto.Items.Select(ItemMapper.FromDto).ToList();

            // Use your Inventory factory method
            return Domain.Entities.Inventory.Inventory.Create(
                dto.Capacity,
                dto.Gold,
                items);
        }
    }
}

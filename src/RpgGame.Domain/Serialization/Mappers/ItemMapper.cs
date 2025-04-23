using RpgGame.Application.Serialization.DTOs;
using RpgGame.Domain.Interfaces.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Serialization.Mappers
{
    public static class ItemMapper
    {
        public static ItemDto ToDto(IItem item)
        {
            var dto = new ItemDto
            {
                Name = item.Name,
                Description = item.Description,
                Value = item.Value,
                Type = item.Type,
                ItemClass = item.GetType().Name
            };

            // Handle equipment type items
            if (item is IEquipment equipment)
            {
                dto.Slot = equipment.Slot;
                dto.BonusValue = equipment.BonusValue;
            }

            // Handle consumable type items with reflection to get specific properties
            if (item is IConsumable)
            {
                // You'll need specific handling for different consumable types
                // For example, for health potions:
                if (item.GetType().Name == "HealthPotion")
                {
                    // Use reflection to get private heal amount field
                    var healAmountField = item.GetType().GetField("_healAmount",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    if (healAmountField != null)
                    {
                        dto.HealAmount = (int)healAmountField.GetValue(item);
                    }
                }
                else if (item.GetType().Name == "ManaPotion")
                {
                    var manaAmountField = item.GetType().GetField("_manaAmount",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    if (manaAmountField != null)
                    {
                        dto.ManaAmount = (int)manaAmountField.GetValue(item);
                    }
                }
            }

            return dto;
        }

        public static IItem FromDto(ItemDto dto)
        {
            // This implementation will depend on your item factory or creation methods
            // You would need to create the appropriate item type based on dto.ItemClass

            // Example (assuming you have factories or public constructors):
            switch (dto.ItemClass)
            {
                //case "HealthPotion":
                //    return new HealthPotion(dto.Name, dto.Description, dto.Value, dto.HealAmount ?? 0);
                //
                //case "ManaPotion":
                //    return new ManaPotion(dto.Name, dto.Description, dto.Value, dto.ManaAmount ?? 0);
                //
                //case "Sword":
                //    return new Sword(dto.Name, dto.Description, dto.Value, dto.BonusValue ?? 0);

                // Add other item types as needed

                default:
                    throw new NotImplementedException($"Item type {dto.ItemClass} not implemented for deserialization");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.WebApi.DTOs.Responses
{
    public class ItemTemplateResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemType { get; set; }
        public int Value { get; set; }
        public Dictionary<string, int> StatModifiers { get; set; }
        public bool IsConsumable { get; set; }
        public bool IsEquippable { get; set; }
        public string? EquipmentSlot { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
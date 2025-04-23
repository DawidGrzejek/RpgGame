using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Serialization.DTOs
{
    [Serializable]
    public class InventoryDto
    {
        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
        public int Capacity { get; set; }
        public int Gold { get; set; }
    }
}

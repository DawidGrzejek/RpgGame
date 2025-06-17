using MediatR;
using RpgGame.Application.Serialization.DTOs;

namespace RpgGame.Application.Commands.Inventory
{
    /// <summary>  
    /// Command to use an item from a character's inventory.  
    /// </summary>  
    public class UseItemCommand : IRequest<CommandResult>
    {
        /// <summary>  
        /// The unique identifier of the character.  
        /// </summary>  
        public Guid CharacterId { get; set; }

        /// <summary>  
        /// The unique identifier of the item to use.  
        /// </summary>  
        public Guid ItemId { get; set; }
    }
}

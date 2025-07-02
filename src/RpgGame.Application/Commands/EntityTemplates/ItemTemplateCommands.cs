using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;

namespace RpgGame.Application.Commands.EntityTemplates
{
    public record CreateItemTemplateCommand(
        string Name,
        string Description,
        ItemType ItemType,
        int Value,
        Dictionary<string, int> StatModifiers,
        bool IsConsumable,
        bool IsEquippable,
        EquipmentSlot? EquipmentSlot
    ) : IRequest<ItemTemplate>;

    public record UpdateItemTemplateCommand(
        Guid Id,
        string Name,
        string Description,
        ItemType ItemType,
        int Value,
        Dictionary<string, int> StatModifiers,
        bool IsConsumable,
        bool IsEquippable,
        EquipmentSlot? EquipmentSlot
    ) : IRequest<ItemTemplate>;

    public record DeleteItemTemplateCommand(Guid Id) : IRequest<bool>;
}
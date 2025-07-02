using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RpgGame.Domain.Entities.Configuration;

namespace RpgGame.Application.Queries.EntityTemplates
{
    public record GetAllItemTemplatesQuery() : IRequest<IEnumerable<ItemTemplate>>;
    
    public record GetItemTemplateByIdQuery(Guid Id) : IRequest<ItemTemplate>;
    
    public record GetItemTemplatesByTypeQuery(string ItemType) : IRequest<IEnumerable<ItemTemplate>>;
}
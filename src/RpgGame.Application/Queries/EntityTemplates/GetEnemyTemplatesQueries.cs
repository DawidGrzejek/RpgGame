using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RpgGame.Domain.Entities.Configuration;

namespace RpgGame.Application.Queries.EntityTemplates
{
    public record GetAllEnemyTemplatesQuery() : IRequest<IEnumerable<EnemyTemplate>>;
    
    public record GetEnemyTemplateByIdQuery(Guid Id) : IRequest<EnemyTemplate>;
    
    public record GetEnemyTemplatesByTypeQuery(string EnemyType) : IRequest<IEnumerable<EnemyTemplate>>;
}
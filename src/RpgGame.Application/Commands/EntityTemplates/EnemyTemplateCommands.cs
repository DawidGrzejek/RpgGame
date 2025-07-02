using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;

namespace RpgGame.Application.Commands.EntityTemplates
{
    public record CreateEnemyTemplateCommand(
        string Name,
        string Description,
        int BaseHealth,
        int BaseStrength,
        int BaseDefense,
        int ExperienceReward,
        EnemyType EnemyType,
        List<string> PossibleLoot,
        Dictionary<string, object> SpecialAbilities
    ) : IRequest<EnemyTemplate>;
    
    public record UpdateEnemyTemplateCommand(
        Guid Id,
        string Name,
        string Description,
        int BaseHealth,
        int BaseStrength,
        int BaseDefense,
        int ExperienceReward,
        EnemyType EnemyType,
        List<string> PossibleLoot,
        Dictionary<string, object> SpecialAbilities
    ) : IRequest<EnemyTemplate>;

    public record DeleteEnemyTemplateCommand(Guid Id) : IRequest<bool>;
}
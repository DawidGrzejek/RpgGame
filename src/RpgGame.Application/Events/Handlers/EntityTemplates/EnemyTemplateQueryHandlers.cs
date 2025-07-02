using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RpgGame.Application.Queries.EntityTemplates;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Repositories;

namespace RpgGame.Application.Events.Handlers.EntityTemplates
{
    public class GetAllEnemyTemplatesQueryHandler : IRequestHandler<GetAllEnemyTemplatesQuery, IEnumerable<EnemyTemplate>>
    {
        private readonly IEnemyTemplateRepository _repository;

        public GetAllEnemyTemplatesQueryHandler(IEnemyTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EnemyTemplate>> Handle(GetAllEnemyTemplatesQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }

    public class GetEnemyTemplateByIdQueryHandler : IRequestHandler<GetEnemyTemplateByIdQuery, EnemyTemplate>
    {
        private readonly IEnemyTemplateRepository _repository;

        public GetEnemyTemplateByIdQueryHandler(IEnemyTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<EnemyTemplate> Handle(GetEnemyTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id);
        }
    }

    public class GetEnemyTemplatesByTypeQueryHandler : IRequestHandler<GetEnemyTemplatesByTypeQuery, IEnumerable<EnemyTemplate>>
    {
        private readonly IEnemyTemplateRepository _repository;

        public GetEnemyTemplatesByTypeQueryHandler(IEnemyTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EnemyTemplate>> Handle(GetEnemyTemplatesByTypeQuery request, CancellationToken cancellationToken)
        {
            if (Enum.TryParse<EnemyType>(request.EnemyType, out var enemyType))
            {
                var result = await _repository.GetByTypeAsync(enemyType);
                if (result.Succeeded)
                {
                    return result.Data ?? [];
                }
                else
                {
                    throw new Exception($"Error retrieving enemy templates by type: {result.Errors}");
                }
            }
            return [];
        }
    }
}
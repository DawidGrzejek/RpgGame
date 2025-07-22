using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RpgGame.Application.Commands.EntityTemplates;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Interfaces.Repositories;

namespace RpgGame.Application.Events.Handlers.EntityTemplates
{
    public class CreateEnemyTemplateCommandHandler : IRequestHandler<CreateEnemyTemplateCommand, EnemyTemplate>
    {
        private readonly IEnemyTemplateRepository _repository;

        public CreateEnemyTemplateCommandHandler(IEnemyTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<EnemyTemplate> Handle(CreateEnemyTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new EnemyTemplate(
                request.Name,
                request.Description,
                request.BaseHealth,
                request.BaseStrength,
                request.BaseDefense,
                request.ExperienceReward,
                request.EnemyType
            );

            // Add loot items
            foreach (var lootItem in request.PossibleLoot)
            {
                template.AddLootItem(lootItem);
            }

            // Add special abilities
            foreach (var ability in request.SpecialAbilities)
            {
                template.AddSpecialAbility(ability.Key, ability.Value);
            }

            await _repository.AddAsync(template);
            await _repository.SaveChangesAsync();

            return template;
        }
    }

    public class UpdateEnemyTemplateCommandHandler : IRequestHandler<UpdateEnemyTemplateCommand, EnemyTemplate>
    {
        private readonly IEnemyTemplateRepository _repository;

        public UpdateEnemyTemplateCommandHandler(IEnemyTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<EnemyTemplate> Handle(UpdateEnemyTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id);
            if (template == null)
                return null;

            // Update template properties
            template.UpdateDetails(
                request.Name,
                request.Description,
                request.BaseHealth,
                request.BaseStrength,
                request.BaseDefense,
                request.ExperienceReward,
                request.EnemyType
            );

            // Clear and re-add loot items
            template.ClearLoot();
            foreach (var lootItem in request.PossibleLoot)
            {
                template.AddLootItem(lootItem);
            }

            // Clear and re-add special abilities
            template.ClearSpecialAbilities();
            foreach (var ability in request.SpecialAbilities)
            {
                template.AddSpecialAbility(ability.Key, ability.Value);
            }

            await _repository.UpdateAsync(template);
            await _repository.SaveChangesAsync();

            return template;
        }
    }

    public class DeleteEnemyTemplateCommandHandler : IRequestHandler<DeleteEnemyTemplateCommand, bool>
    {
        private readonly IEnemyTemplateRepository _repository;

        public DeleteEnemyTemplateCommandHandler(IEnemyTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteEnemyTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id);
            if (template == null)
                return false;

            await _repository.DeleteAsync(template);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
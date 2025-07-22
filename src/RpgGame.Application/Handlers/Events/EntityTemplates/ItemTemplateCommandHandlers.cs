using MediatR;
using RpgGame.Application.Commands.EntityTemplates;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Interfaces.Repositories;

namespace RpgGame.Application.Events.Handlers.EntityTemplates
{
    public class CreateItemTemplateCommandHandler : IRequestHandler<CreateItemTemplateCommand, ItemTemplate>
    {
        private readonly IItemTemplateRepository _repository;

        public CreateItemTemplateCommandHandler(IItemTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<ItemTemplate> Handle(CreateItemTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new ItemTemplate(
                request.Name,
                request.Description,
                request.ItemType,
                request.Value,
                request.IsConsumable,
                request.IsEquippable,
                request.EquipmentSlot
            );

            // Add stat modifiers
            foreach (var modifier in request.StatModifiers)
            {
                template.AddStatModifier(modifier.Key, modifier.Value);
            }

            await _repository.AddAsync(template);
            await _repository.SaveChangesAsync();

            return template;
        }
    }

    public class UpdateItemTemplateCommandHandler : IRequestHandler<UpdateItemTemplateCommand, ItemTemplate>
    {
        private readonly IItemTemplateRepository _repository;

        public UpdateItemTemplateCommandHandler(IItemTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<ItemTemplate> Handle(UpdateItemTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id);
            if (template == null)
                return null;

            // Update template properties
            template.UpdateDetails(
                request.Name,
                request.Description,
                request.ItemType,
                request.Value,
                request.IsConsumable,
                request.IsEquippable,
                request.EquipmentSlot
            );

            // Clear and re-add stat modifiers
            template.ClearStatModifiers();
            foreach (var modifier in request.StatModifiers)
            {
                template.AddStatModifier(modifier.Key, modifier.Value);
            }

            await _repository.UpdateAsync(template);
            await _repository.SaveChangesAsync();

            return template;
        }
    }

    public class DeleteItemTemplateCommandHandler : IRequestHandler<DeleteItemTemplateCommand, bool>
    {
        private readonly IItemTemplateRepository _repository;

        public DeleteItemTemplateCommandHandler(IItemTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteItemTemplateCommand request, CancellationToken cancellationToken)
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
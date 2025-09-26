using MediatR;
using RpgGame.Application.Commands.EntityTemplates;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Interfaces.Repositories;

namespace RpgGame.Application.Events.Handlers.EntityTemplates
{
    public class CreateItemTemplateCommandHandler : IRequestHandler<CreateItemTemplateCommand, ItemTemplate>
    {
        private readonly IItemTemplateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateItemTemplateCommandHandler(IItemTemplateRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.CommitAsync(cancellationToken);

            return template;
        }
    }

    public class UpdateItemTemplateCommandHandler : IRequestHandler<UpdateItemTemplateCommand, ItemTemplate>
    {
        private readonly IItemTemplateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateItemTemplateCommandHandler(IItemTemplateRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ItemTemplate> Handle(UpdateItemTemplateCommand request, CancellationToken cancellationToken)
        {
            var templateResult = await _repository.GetByIdAsync(request.Id);
            if (!templateResult.Succeeded || templateResult.Data == null)
                return null;

            var template = templateResult.Data;

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
            await _unitOfWork.CommitAsync(cancellationToken);

            return template;
        }
    }

    public class DeleteItemTemplateCommandHandler : IRequestHandler<DeleteItemTemplateCommand, bool>
    {
        private readonly IItemTemplateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteItemTemplateCommandHandler(IItemTemplateRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteItemTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id);
            if (template == null)
                return false;

            await _repository.DeleteAsync(template);
            await _unitOfWork.CommitAsync(cancellationToken);

            return true;
        }
    }
}
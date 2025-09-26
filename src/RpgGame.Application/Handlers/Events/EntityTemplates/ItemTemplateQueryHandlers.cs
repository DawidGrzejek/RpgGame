using MediatR;
using RpgGame.Application.Queries.EntityTemplates;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Repositories;

namespace RpgGame.Application.Events.Handlers.EntityTemplates
{
    public class GetAllItemTemplatesQueryHandler : IRequestHandler<GetAllItemTemplatesQuery, IEnumerable<ItemTemplate>>
    {
        private readonly IItemTemplateRepository _repository;

        public GetAllItemTemplatesQueryHandler(IItemTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ItemTemplate>> Handle(GetAllItemTemplatesQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAsync();
            return result.Data ?? Enumerable.Empty<ItemTemplate>();
        }
    }

    public class GetItemTemplateByIdQueryHandler : IRequestHandler<GetItemTemplateByIdQuery, ItemTemplate>
    {
        private readonly IItemTemplateRepository _repository;

        public GetItemTemplateByIdQueryHandler(IItemTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<ItemTemplate> Handle(GetItemTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id);
        }
    }

    public class GetItemTemplatesByTypeQueryHandler : IRequestHandler<GetItemTemplatesByTypeQuery, IEnumerable<ItemTemplate>>
    {
        private readonly IItemTemplateRepository _repository;

        public GetItemTemplatesByTypeQueryHandler(IItemTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ItemTemplate>> Handle(GetItemTemplatesByTypeQuery request, CancellationToken cancellationToken)
        {
            //throw notimplemnetedexception 
            throw new NotImplementedException("This method is not implemented yet.");


            // if (Enum.TryParse<ItemType>(request.ItemType, out var itemType))
            // {
            //     return await _repository.GetByTypeAsync(itemType);
            // }
            // return Enumerable.Empty<ItemTemplate>();
        }
    }
}
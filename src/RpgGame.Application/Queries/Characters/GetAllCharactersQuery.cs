using MediatR;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Characters.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Queries.Characters
{
    public class GetAllCharactersQuery : IQuery<OperationResult<IReadOnlyList<Character>>>
    {
        // This query doesn't need any parameters
    }

    public class GetAllCharactersQueryHandler : IRequestHandler<GetAllCharactersQuery, OperationResult<IReadOnlyList<Character>>>
    {
        private readonly ICharacterRepository _characterRepository;

        public GetAllCharactersQueryHandler(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        public async Task<OperationResult<IReadOnlyList<Character>>> Handle(GetAllCharactersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var charactersResult = await _characterRepository.GetAllAsync();
                if (!charactersResult.Succeeded)
                {
                    return OperationResult<IReadOnlyList<Character>>.Failure(charactersResult.Errors);
                }

                return OperationResult<IReadOnlyList<Character>>.Success(charactersResult.Data);
            }
            catch (System.Exception ex)
            {
                return OperationResult<IReadOnlyList<Character>>.Failure("GetAllCharactersQueryHandlerException", $"Error retrieving characters: {ex.Message} - {ex.InnerException}");
            }
        }
    }
}
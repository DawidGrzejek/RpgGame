using MediatR;
using RpgGame.Application.Repositories;
using RpgGame.Domain.Entities.Characters.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Queries.Characters
{
    public class GetAllCharactersQuery : IQuery<QueryResult<IReadOnlyList<Character>>>
    {
        // This query doesn't need any parameters
    }

    public class GetAllCharactersQueryHandler : IRequestHandler<GetAllCharactersQuery, QueryResult<IReadOnlyList<Character>>>
    {
        private readonly ICharacterRepository _characterRepository;

        public GetAllCharactersQueryHandler(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        public async Task<QueryResult<IReadOnlyList<Character>>> Handle(GetAllCharactersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var characters = await _characterRepository.GetAllAsync();
                return QueryResult<IReadOnlyList<Character>>.Ok(characters);
            }
            catch (System.Exception ex)
            {
                return QueryResult<IReadOnlyList<Character>>.Fail($"Error retrieving characters: {ex.Message}");
            }
        }
    }
}
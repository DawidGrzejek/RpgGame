using RpgGame.Application.Events;
using RpgGame.Application.Repositories;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.Infrastructure.Persistence.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly GameDbContext _dbContext;
        private readonly IEventStoreRepository _eventStore;

        public CharacterRepository(GameDbContext dbContext, IEventStoreRepository eventStore)
        {
            _dbContext = dbContext;
            _eventStore = eventStore;
        }

        public async Task<Guid> AddAsync(Character character)
        {
            // Store events from the character
            var events = character.DomainEvents.ToList();
            if (events.Any())
            {
                await _eventStore.SaveEventsAsync(events);
                character.ClearDomainEvents();
            }

            return character.Id;
        }

        public async Task<Character> GetByIdAsync(Guid id)
        {
            // Retrieve all events for this character
            var events = await _eventStore.GetEventsAsync(id);

            if (!events.Any())
                return null;

            // Reconstruct character from events
            return Character.FromEvents(id, events);
        }

        public async Task<IReadOnlyList<Character>> GetAllAsync()
        {
            // This is more complex in an event-sourced system
            // We need to query for unique aggregate IDs from the event store

            // Get unique character IDs from CharacterCreatedEvent
            var characterIds = _dbContext.Events
                .Where(e => e.EventType == "CharacterCreatedEvent")
                .Select(e => e.AggregateId)
                .Distinct()
                .ToList();

            var characters = new List<Character>();

            // Reconstruct each character from its event stream
            foreach (var id in characterIds)
            {
                var character = await GetByIdAsync(id);
                if (character != null)
                {
                    characters.Add(character);
                }
            }

            return characters;
        }

        public async Task UpdateAsync(Character character)
        {
            // In event sourcing, just save the new events
            var events = character.DomainEvents.ToList();
            if (events.Any())
            {
                await _eventStore.SaveEventsAsync(events);
                character.ClearDomainEvents();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            // In a true event-sourced system, we don't delete events
            // Instead, we'd add a "CharacterDeletedEvent"

            // For now, we'll just add a deleted flag in future events
            throw new NotImplementedException("Deletion is not supported in event-sourced architecture");
        }
    }
}
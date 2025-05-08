using RpgGame.Domain.Enums;
using RpgGame.Domain.Events.Base;
using System;
using System.Xml.Linq;

namespace RpgGame.Domain.Events.Characters
{
    public class CharacterCreatedEvent : DomainEventBase
    {
        public string Name { get; }
        public CharacterType CharacterType { get; }

        public CharacterCreatedEvent(
            Guid aggregateId,
            int version,
            string name,
            CharacterType characterType) : base(aggregateId, version)
        {
            Name = name;
            CharacterType = characterType;
        }
    }
}
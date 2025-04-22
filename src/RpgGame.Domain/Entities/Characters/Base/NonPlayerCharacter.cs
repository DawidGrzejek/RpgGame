using RpgGame.Domain.Interfaces.Characters;

namespace RpgGame.Domain.Entities.Characters.Base
{
    /// <summary>
    /// Base class for all non-player characters
    /// </summary>
    public abstract class NonPlayerCharacter : Character, INonPlayerCharacter
    {
        // Encapsulated fields
        protected bool _isFriendly;
        protected string _dialogue;

        // Properties
        public bool IsFriendly => _isFriendly;
        public string Dialogue => _dialogue;

        /// <summary>
        /// Constructor for non-player characters
        /// </summary>
        protected NonPlayerCharacter(
            string name,
            int health,
            int strength,
            int defense,
            bool isFriendly,
            string dialogue)
            : base(name, health, strength, defense)
        {
            _isFriendly = isFriendly;
            _dialogue = dialogue ?? string.Empty;
        }

        /// <summary>
        /// Defines interaction behavior with player characters
        /// </summary>
        public virtual void Interact(IPlayerCharacter player)
        {
            if (string.IsNullOrEmpty(_dialogue))
                return;

            Console.WriteLine($"{Name} says: \"{_dialogue}\"");
        }
    }
}
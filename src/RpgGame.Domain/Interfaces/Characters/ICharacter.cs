namespace RpgGame.Domain.Interfaces.Characters
{
    /// <summary>
    /// Base interface for all characters in the game
    /// </summary>
    public interface ICharacter
    {
        string Name { get; }
        int Health { get; }
        int MaxHealth { get; }
        int Level { get; }
        bool IsAlive { get; }

        void Attack(ICharacter target);
        void TakeDamage(int damage);
        void Heal(int amount);
        void LevelUp();
    }
}
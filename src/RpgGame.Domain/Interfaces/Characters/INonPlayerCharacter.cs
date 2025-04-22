namespace RpgGame.Domain.Interfaces.Characters
{
    /// <summary>
    /// Interface for non-player characters (NPCs)
    /// </summary>
    public interface INonPlayerCharacter : ICharacter
    {
        bool IsFriendly { get; }
        string Dialogue { get; }

        void Interact(IPlayerCharacter player);
    }
}
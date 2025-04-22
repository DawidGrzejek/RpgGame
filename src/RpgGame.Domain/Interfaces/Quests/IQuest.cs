using System.Collections.Generic;
using RpgGame.Domain.Interfaces.Characters;
using RpgGame.Domain.Interfaces.Items;

namespace RpgGame.Domain.Interfaces.Quests
{
    /// <summary>
    /// Interface for game quests
    /// </summary>
    public interface IQuest
    {
        string Name { get; }
        string Description { get; }
        bool IsActive { get; }
        bool IsCompleted { get; }
        int ExperienceReward { get; }
        int GoldReward { get; }
        IReadOnlyList<IItem> ItemRewards { get; }

        void StartQuest();
        void UpdateObjective(string objectiveId, int progress);
        bool CheckCompletion();
        void CompleteQuest(IPlayerCharacter player);
    }
}
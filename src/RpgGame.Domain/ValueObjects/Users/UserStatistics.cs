// Domain/ValueObjects/Users/UserStatistics.cs
using RpgGame.Domain.Base;

namespace RpgGame.Domain.ValueObjects.Users
{
    /// <summary>
    /// Value object representing user's game statistics
    /// Immutable object that tracks user's gameplay metrics
    /// </summary>
    public class UserStatistics : DomainEntity
    {
        public int TotalPlayTimeMinutes { get; private set; }
        public int CharactersCreated { get; private set; }
        public int TotalLogins { get; private set; }
        public int HighestCharacterLevel { get; private set; }
        public int TotalEnemiesDefeated { get; private set; }
        public int TotalQuestsCompleted { get; private set; }
        public int TotalDeaths { get; private set; }
        public DateTime FirstLoginDate { get; private set; }
        public DateTime? LastActiveDate { get; private set; }
        
        // Achievements/Milestones
        public bool HasCreatedFirstCharacter { get; private set; }
        public bool HasReachedLevel10 { get; private set; }
        public bool HasReachedLevel50 { get; private set; }
        public bool HasCompleted10Quests { get; private set; }
        public bool HasDefeated100Enemies { get; private set; }

        private UserStatistics() { } // EF Constructor

        public UserStatistics(
            int totalPlayTimeMinutes = 0,
            int charactersCreated = 0,
            int totalLogins = 0,
            int highestCharacterLevel = 0,
            int totalEnemiesDefeated = 0,
            int totalQuestsCompleted = 0,
            int totalDeaths = 0,
            DateTime? firstLoginDate = null,
            DateTime? lastActiveDate = null,
            bool hasCreatedFirstCharacter = false,
            bool hasReachedLevel10 = false,
            bool hasReachedLevel50 = false,
            bool hasCompleted10Quests = false,
            bool hasDefeated100Enemies = false)
        {
            TotalPlayTimeMinutes = totalPlayTimeMinutes;
            CharactersCreated = charactersCreated;
            TotalLogins = totalLogins;
            HighestCharacterLevel = highestCharacterLevel;
            TotalEnemiesDefeated = totalEnemiesDefeated;
            TotalQuestsCompleted = totalQuestsCompleted;
            TotalDeaths = totalDeaths;
            FirstLoginDate = firstLoginDate ?? DateTime.UtcNow;
            LastActiveDate = lastActiveDate;
            HasCreatedFirstCharacter = hasCreatedFirstCharacter;
            HasReachedLevel10 = hasReachedLevel10;
            HasReachedLevel50 = hasReachedLevel50;
            HasCompleted10Quests = hasCompleted10Quests;
            HasDefeated100Enemies = hasDefeated100Enemies;
        }

        /// <summary>
        /// Creates initial statistics for a new user
        /// </summary>
        public static UserStatistics Initial() => new(firstLoginDate: DateTime.UtcNow);

        /// <summary>
        /// Records a login event
        /// </summary>
        public UserStatistics RecordLogin()
        {
            return new UserStatistics(
                TotalPlayTimeMinutes,
                CharactersCreated,
                TotalLogins + 1,
                HighestCharacterLevel,
                TotalEnemiesDefeated,
                TotalQuestsCompleted,
                TotalDeaths,
                FirstLoginDate,
                DateTime.UtcNow,
                HasCreatedFirstCharacter,
                HasReachedLevel10,
                HasReachedLevel50,
                HasCompleted10Quests,
                HasDefeated100Enemies
            );
        }

        /// <summary>
        /// Records character creation
        /// </summary>
        public UserStatistics RecordCharacterCreation()
        {
            return new UserStatistics(
                TotalPlayTimeMinutes,
                CharactersCreated + 1,
                TotalLogins,
                HighestCharacterLevel,
                TotalEnemiesDefeated,
                TotalQuestsCompleted,
                TotalDeaths,
                FirstLoginDate,
                LastActiveDate,
                true, // First character created
                HasReachedLevel10,
                HasReachedLevel50,
                HasCompleted10Quests,
                HasDefeated100Enemies
            );
        }

        /// <summary>
        /// Updates highest character level achieved
        /// </summary>
        public UserStatistics UpdateHighestLevel(int newLevel)
        {
            var currentHighest = Math.Max(HighestCharacterLevel, newLevel);
            
            return new UserStatistics(
                TotalPlayTimeMinutes,
                CharactersCreated,
                TotalLogins,
                currentHighest,
                TotalEnemiesDefeated,
                TotalQuestsCompleted,
                TotalDeaths,
                FirstLoginDate,
                LastActiveDate,
                HasCreatedFirstCharacter,
                currentHighest >= 10, // Achievement check
                currentHighest >= 50, // Achievement check
                HasCompleted10Quests,
                HasDefeated100Enemies
            );
        }

        /// <summary>
        /// Records enemy defeat
        /// </summary>
        public UserStatistics RecordEnemyDefeated()
        {
            var newCount = TotalEnemiesDefeated + 1;
            
            return new UserStatistics(
                TotalPlayTimeMinutes,
                CharactersCreated,
                TotalLogins,
                HighestCharacterLevel,
                newCount,
                TotalQuestsCompleted,
                TotalDeaths,
                FirstLoginDate,
                LastActiveDate,
                HasCreatedFirstCharacter,
                HasReachedLevel10,
                HasReachedLevel50,
                HasCompleted10Quests,
                newCount >= 100 // Achievement check
            );
        }

        /// <summary>
        /// Records quest completion
        /// </summary>
        public UserStatistics RecordQuestCompleted()
        {
            var newCount = TotalQuestsCompleted + 1;
            
            return new UserStatistics(
                TotalPlayTimeMinutes,
                CharactersCreated,
                TotalLogins,
                HighestCharacterLevel,
                TotalEnemiesDefeated,
                newCount,
                TotalDeaths,
                FirstLoginDate,
                LastActiveDate,
                HasCreatedFirstCharacter,
                HasReachedLevel10,
                HasReachedLevel50,
                newCount >= 10, // Achievement check
                HasDefeated100Enemies
            );
        }

        /// <summary>
        /// Records character death
        /// </summary>
        public UserStatistics RecordDeath()
        {
            return new UserStatistics(
                TotalPlayTimeMinutes,
                CharactersCreated,
                TotalLogins,
                HighestCharacterLevel,
                TotalEnemiesDefeated,
                TotalQuestsCompleted,
                TotalDeaths + 1,
                FirstLoginDate,
                LastActiveDate,
                HasCreatedFirstCharacter,
                HasReachedLevel10,
                HasReachedLevel50,
                HasCompleted10Quests,
                HasDefeated100Enemies
            );
        }

        /// <summary>
        /// Adds play time
        /// </summary>
        public UserStatistics AddPlayTime(int minutes)
        {
            return new UserStatistics(
                TotalPlayTimeMinutes + minutes,
                CharactersCreated,
                TotalLogins,
                HighestCharacterLevel,
                TotalEnemiesDefeated,
                TotalQuestsCompleted,
                TotalDeaths,
                FirstLoginDate,
                DateTime.UtcNow, // Update last active
                HasCreatedFirstCharacter,
                HasReachedLevel10,
                HasReachedLevel50,
                HasCompleted10Quests,
                HasDefeated100Enemies
            );
        }

        /// <summary>
        /// Gets total play time as TimeSpan
        /// </summary>
        public TimeSpan GetTotalPlayTime() => TimeSpan.FromMinutes(TotalPlayTimeMinutes);

        /// <summary>
        /// Gets days since first login
        /// </summary>
        public int GetDaysSinceFirstLogin() => (DateTime.UtcNow - FirstLoginDate).Days;

        /// <summary>
        /// Gets all unlocked achievements
        /// </summary>
        public List<string> GetUnlockedAchievements()
        {
            var achievements = new List<string>();
            
            if (HasCreatedFirstCharacter) achievements.Add("First Character Created");
            if (HasReachedLevel10) achievements.Add("Reached Level 10");
            if (HasReachedLevel50) achievements.Add("Reached Level 50");
            if (HasCompleted10Quests) achievements.Add("Quest Master (10 Quests)");
            if (HasDefeated100Enemies) achievements.Add("Enemy Slayer (100 Enemies)");
            
            return achievements;
        }

        /// <summary>
        /// Calculates user engagement level
        /// </summary>
        public UserEngagementLevel GetEngagementLevel()
        {
            var score = 0;
            
            score += TotalLogins; // 1 point per login
            score += CharactersCreated * 5; // 5 points per character
            score += TotalQuestsCompleted * 3; // 3 points per quest
            score += TotalEnemiesDefeated; // 1 point per enemy
            score += HighestCharacterLevel * 2; // 2 points per level
            
            return score switch
            {
                < 50 => UserEngagementLevel.Beginner,
                < 200 => UserEngagementLevel.Casual,
                < 500 => UserEngagementLevel.Regular,
                < 1000 => UserEngagementLevel.Dedicated,
                _ => UserEngagementLevel.Hardcore
            };
        }

        /// <summary>
        /// Value object equality
        /// </summary>
        public override bool Equals(object? obj)
        {
            return obj is UserStatistics other &&
                   TotalPlayTimeMinutes == other.TotalPlayTimeMinutes &&
                   CharactersCreated == other.CharactersCreated &&
                   TotalLogins == other.TotalLogins &&
                   HighestCharacterLevel == other.HighestCharacterLevel &&
                   TotalEnemiesDefeated == other.TotalEnemiesDefeated &&
                   TotalQuestsCompleted == other.TotalQuestsCompleted &&
                   TotalDeaths == other.TotalDeaths;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                TotalPlayTimeMinutes,
                CharactersCreated,
                TotalLogins,
                HighestCharacterLevel,
                TotalEnemiesDefeated,
                TotalQuestsCompleted,
                TotalDeaths
            );
        }
    }

    /// <summary>
    /// Enum representing user engagement levels
    /// </summary>
    public enum UserEngagementLevel
    {
        Beginner,
        Casual,
        Regular,
        Dedicated,
        Hardcore
    }
}
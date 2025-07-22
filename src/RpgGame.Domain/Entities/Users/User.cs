using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RpgGame.Domain.Base;
using RpgGame.Domain.Events.Users;
using RpgGame.Domain.ValueObjects.Users;

namespace RpgGame.Domain.Entities.Users
{
    /// <summary>
    /// Domain User entity - represents the business/game-specific user information.
    /// Linked to ASP.NET Identity via AspNetUserId for clean separation of concerns.
    /// </summary>
    public class User : DomainEntity
    {
        public string AspNetUserId { get; private set; } //Link to IdentityUser
        public string Username { get; private set; }
        public string Email { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public List<string> Roles { get; private set; } = new();
        public bool IsActive { get; private set; } = true;

        // Domain-specific properties (game-related)
        public UserPreferences Preferences { get; private set; }
        public List<Guid> CharacterIds { get; private set; } = new();
        public UserStatistics Statistics { get; private set; }

        private User() { } // EF Constructor

        /// <summary>
        /// Factory method to create a new User instance.
        /// This method encapsulates the creation logic and ensures that all required properties are set.
        /// </summary>
        /// <param name="aspNetUserId">The ASP.NET Identity user ID.</param>
        /// <param name="username">The username for the user.</param>
        /// <param name="email">The email address of the user.</param>
        /// <returns>A new User instance.</returns>
        public static User Create(string aspNetUserId, string username, string email)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                AspNetUserId = aspNetUserId,
                Username = username,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                Preferences = UserPreferences.Default(),
                Statistics = UserStatistics.Initial()
            };

            //user.AddDomainEvent(new UserRegisteredEvent(user.Id, username, email));
            return user;
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            //AddDomainEvent(new UserLoggedInEvent(Id, Username));
        }

        public void AddRole(string role)
        {
            if (!Roles.Contains(role))
            {
                Roles.Add(role);
                //AddDomainEvent(new UserRoleAddedEvent(Id, role));
            }
        }

        public void AddCharacter(Guid characterId, string characterName, string characterType)
        {
            if (!CharacterIds.Contains(characterId))
            {
                CharacterIds.Add(characterId);
                
                // Update statistics
                var updatedStats = Statistics.RecordCharacterCreation();
                Statistics = updatedStats;
                
                AddDomainEvent(new CharacterAssignedToUserEvent(Id, characterId, characterName, characterType, Username));
                
                // Check for achievements
                if (updatedStats.HasCreatedFirstCharacter && updatedStats.CharactersCreated == 1)
                {
                    AddDomainEvent(new UserAchievementUnlockedEvent(
                        Id, Username, "First Character Created", 
                        "Created your first character and started your adventure!", 
                        "Character Creation"));
                }
            }
        }


        public void RemoveCharacter(Guid characterId, string characterName, string reason = "Character deleted")
        {
            if (CharacterIds.Remove(characterId))
            {
                AddDomainEvent(new CharacterRemovedFromUserEvent(Id, characterId, characterName, Username, reason));
            }
        }

        public void Deactivate(string reason, string deactivatedBy, bool isTemporary = false, DateTime? reactivationDate = null)
        {
            IsActive = false;
            AddDomainEvent(new UserDeactivatedEvent(Id, Username, Email, reason, deactivatedBy, isTemporary, reactivationDate));
        }

        public void Reactivate(string reactivatedBy, string reason = "Account reactivated")
        {
            IsActive = true;
            AddDomainEvent(new UserReactivatedEvent(Id, Username, Email, reactivatedBy, reason));
        }

        public void UpdatePreferences(UserPreferences newPreferences, string updatedBy = "User")
        {
            var oldPreferences = PreferencesToDictionary(Preferences);
            var newPreferencesDict = PreferencesToDictionary(newPreferences);
            
            Preferences = newPreferences;
            AddDomainEvent(new UserPreferencesUpdatedEvent(Id, Username, oldPreferences, newPreferencesDict));
        }

        public void UpdateStatistics(UserStatistics newStatistics)
        {
            var oldEngagementLevel = Statistics.GetEngagementLevel();
            Statistics = newStatistics;
            var newEngagementLevel = newStatistics.GetEngagementLevel();
            
            // Check if engagement level increased
            if (newEngagementLevel > oldEngagementLevel)
            {
                AddDomainEvent(new UserAchievementUnlockedEvent(
                    Id, Username, $"{newEngagementLevel} Player", 
                    $"Reached {newEngagementLevel} engagement level!", 
                    "Engagement"));
            }
            
            // Check for level-based achievements
            CheckForLevelAchievements(newStatistics);
            
            AddDomainEvent(new UserStatisticsUpdatedEvent(Id, Username, "General", oldEngagementLevel, newEngagementLevel));
        }

        public void RecordEnemyDefeated()
        {
            var updatedStats = Statistics.RecordEnemyDefeated();
            var oldCount = Statistics.TotalEnemiesDefeated;
            Statistics = updatedStats;
            
            AddDomainEvent(new UserStatisticsUpdatedEvent(Id, Username, "EnemiesDefeated", oldCount, updatedStats.TotalEnemiesDefeated));
            
            // Check for enemy defeat achievements
            if (updatedStats.HasDefeated100Enemies && oldCount < 100)
            {
                AddDomainEvent(new UserAchievementUnlockedEvent(
                    Id, Username, "Enemy Slayer", 
                    "Defeated 100 enemies in combat!", 
                    "Combat"));
            }
        }

        public void RecordQuestCompleted()
        {
            var updatedStats = Statistics.RecordQuestCompleted();
            var oldCount = Statistics.TotalQuestsCompleted;
            Statistics = updatedStats;
            
            AddDomainEvent(new UserStatisticsUpdatedEvent(Id, Username, "QuestsCompleted", oldCount, updatedStats.TotalQuestsCompleted));
            
            // Check for quest achievements
            if (updatedStats.HasCompleted10Quests && oldCount < 10)
            {
                AddDomainEvent(new UserAchievementUnlockedEvent(
                    Id, Username, "Quest Master", 
                    "Completed 10 quests successfully!", 
                    "Questing"));
            }
        }

        public void RecordLevelUp(int newLevel)
        {
            var updatedStats = Statistics.UpdateHighestLevel(newLevel);
            var oldLevel = Statistics.HighestCharacterLevel;
            Statistics = updatedStats;
            
            AddDomainEvent(new UserStatisticsUpdatedEvent(Id, Username, "HighestLevel", oldLevel, newLevel));
            
            CheckForLevelAchievements(updatedStats);
        }

        public void AddPlayTime(int minutes)
        {
            var updatedStats = Statistics.AddPlayTime(minutes);
            Statistics = updatedStats;
            
            // Check for playtime achievements
            CheckForPlayTimeAchievements(updatedStats);
        }

        private void CheckForLevelAchievements(UserStatistics stats)
        {
            if (stats.HasReachedLevel10 && !Statistics.HasReachedLevel10)
            {
                AddDomainEvent(new UserAchievementUnlockedEvent(
                    Id, Username, "Rising Hero", 
                    "Reached level 10 with a character!", 
                    "Character Progression"));
            }
            
            if (stats.HasReachedLevel50 && !Statistics.HasReachedLevel50)
            {
                AddDomainEvent(new UserAchievementUnlockedEvent(
                    Id, Username, "Legendary Adventurer", 
                    "Reached the legendary level 50!", 
                    "Character Progression"));
            }
        }

        private void CheckForPlayTimeAchievements(UserStatistics stats)
        {
            var totalHours = stats.TotalPlayTimeMinutes / 60;
            var previousHours = Statistics.TotalPlayTimeMinutes / 60;
            
            // Check for playtime milestones
            var milestones = new[] { 10, 50, 100, 500, 1000 };
            
            foreach (var milestone in milestones)
            {
                if (totalHours >= milestone && previousHours < milestone)
                {
                    AddDomainEvent(new UserAchievementUnlockedEvent(
                        Id, Username, $"Dedicated Player ({milestone}h)", 
                        $"Played for {milestone} hours total!", 
                        "Playtime"));
                }
            }
        }

        private Dictionary<string, object> PreferencesToDictionary(UserPreferences preferences)
        {
            return new Dictionary<string, object>
            {
                { nameof(preferences.EmailNotifications), preferences.EmailNotifications },
                { nameof(preferences.GameSoundEnabled), preferences.GameSoundEnabled },
                { nameof(preferences.Theme), preferences.Theme },
                { nameof(preferences.Language), preferences.Language }
            };
        }

        // Domain business rules
        public bool CanCreateCharacter(int maxCharactersPerUser = 10)
        {
            return CharacterIds.Count < maxCharactersPerUser && IsActive;
        }

        public bool HasRole(string role)
        {
            return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        public bool IsAdmin()
        {
            return HasRole("Admin");
        }

        public bool IsModerator()
        {
            return HasRole("Admin") || HasRole("Moderator");
        }

        public TimeSpan GetAccountAge()
        {
            return DateTime.UtcNow - CreatedAt;
        }

        public bool IsNewUser()
        {
            return GetAccountAge().TotalDays <= 7;
        }
    }
}
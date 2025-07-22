// Domain/Events/Users/UserEvents.cs
using RpgGame.Domain.Events.Base;

namespace RpgGame.Domain.Events.Users
{
    /// <summary>
    /// Event raised when a new user registers in the system
    /// </summary>
    public class UserRegisteredEvent : DomainEventBase
    {
        public string Username { get; }
        public string Email { get; }
        public string AspNetUserId { get; }
        public DateTime RegistrationDate { get; }

        public UserRegisteredEvent(Guid userId, string username, string email, string aspNetUserId, int version = 1)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            AspNetUserId = aspNetUserId ?? throw new ArgumentNullException(nameof(aspNetUserId));
            RegistrationDate = OccurredAt;
        }
    }

    /// <summary>
    /// Event raised when a user successfully logs into the system
    /// </summary>
    public class UserLoggedInEvent : DomainEventBase
    {
        public string Username { get; }
        public string Email { get; }
        public DateTime LoginTime { get; }
        public string? IpAddress { get; }
        public string? UserAgent { get; }

        public UserLoggedInEvent(Guid userId, string username, string email, int version = 1, 
            string? ipAddress = null, string? userAgent = null)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            LoginTime = OccurredAt;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }
    }

    /// <summary>
    /// Event raised when a role is added to a user
    /// </summary>
    public class UserRoleAddedEvent : DomainEventBase
    {
        public string Username { get; }
        public string Role { get; }
        public string AddedBy { get; }
        public DateTime RoleAssignedAt { get; }

        public UserRoleAddedEvent(Guid userId, string username, string role, string addedBy, int version = 1)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            AddedBy = addedBy ?? throw new ArgumentNullException(nameof(addedBy));
            RoleAssignedAt = OccurredAt;
        }
    }

    /// <summary>
    /// Event raised when a role is removed from a user
    /// </summary>
    public class UserRoleRemovedEvent : DomainEventBase
    {
        public string Username { get; }
        public string Role { get; }
        public string RemovedBy { get; }
        public DateTime RoleRemovedAt { get; }

        public UserRoleRemovedEvent(Guid userId, string username, string role, string removedBy, int version = 1)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            RemovedBy = removedBy ?? throw new ArgumentNullException(nameof(removedBy));
            RoleRemovedAt = OccurredAt;
        }
    }

    /// <summary>
    /// Event raised when a character is assigned to a user
    /// </summary>
    public class CharacterAssignedToUserEvent : DomainEventBase
    {
        public Guid CharacterId { get; }
        public string CharacterName { get; }
        public string CharacterType { get; }
        public string Username { get; }
        public DateTime AssignedAt { get; }

        public CharacterAssignedToUserEvent(Guid userId, Guid characterId, string characterName, 
            string characterType, string username, int version = 1)
            : base(userId, version)
        {
            CharacterId = characterId;
            CharacterName = characterName ?? throw new ArgumentNullException(nameof(characterName));
            CharacterType = characterType ?? throw new ArgumentNullException(nameof(characterType));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            AssignedAt = OccurredAt;
        }
    }

    /// <summary>
    /// Event raised when a character is removed from a user
    /// </summary>
    public class CharacterRemovedFromUserEvent : DomainEventBase
    {
        public Guid CharacterId { get; }
        public string CharacterName { get; }
        public string Username { get; }
        public string RemovalReason { get; }
        public DateTime RemovedAt { get; }

        public CharacterRemovedFromUserEvent(Guid userId, Guid characterId, string characterName, 
            string username, string removalReason, int version = 1)
            : base(userId, version)
        {
            CharacterId = characterId;
            CharacterName = characterName ?? throw new ArgumentNullException(nameof(characterName));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            RemovalReason = removalReason ?? throw new ArgumentNullException(nameof(removalReason));
            RemovedAt = OccurredAt;
        }
    }

    /// <summary>
    /// Event raised when a user account is deactivated
    /// </summary>
    public class UserDeactivatedEvent : DomainEventBase
    {
        public string Username { get; }
        public string Email { get; }
        public string DeactivationReason { get; }
        public string DeactivatedBy { get; }
        public DateTime DeactivatedAt { get; }
        public bool IsTemporary { get; }
        public DateTime? ReactivationDate { get; }

        public UserDeactivatedEvent(Guid userId, string username, string email, string deactivationReason, 
            string deactivatedBy, bool isTemporary = false, DateTime? reactivationDate = null, int version = 1)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            DeactivationReason = deactivationReason ?? throw new ArgumentNullException(nameof(deactivationReason));
            DeactivatedBy = deactivatedBy ?? throw new ArgumentNullException(nameof(deactivatedBy));
            DeactivatedAt = OccurredAt;
            IsTemporary = isTemporary;
            ReactivationDate = reactivationDate;
        }
    }

    /// <summary>
    /// Event raised when a user account is reactivated
    /// </summary>
    public class UserReactivatedEvent : DomainEventBase
    {
        public string Username { get; }
        public string Email { get; }
        public string ReactivatedBy { get; }
        public DateTime ReactivatedAt { get; }
        public string ReactivationReason { get; }

        public UserReactivatedEvent(Guid userId, string username, string email, string reactivatedBy, 
            string reactivationReason, int version = 1)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            ReactivatedBy = reactivatedBy ?? throw new ArgumentNullException(nameof(reactivatedBy));
            ReactivationReason = reactivationReason ?? throw new ArgumentNullException(nameof(reactivationReason));
            ReactivatedAt = OccurredAt;
        }
    }

    /// <summary>
    /// Event raised when user profile information is updated
    /// </summary>
    public class UserProfileUpdatedEvent : DomainEventBase
    {
        public string Username { get; }
        public Dictionary<string, object> ChangedProperties { get; }
        public string UpdatedBy { get; }
        public DateTime UpdatedAt { get; }

        public UserProfileUpdatedEvent(Guid userId, string username, Dictionary<string, object> changedProperties, 
            string updatedBy, int version = 1)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            ChangedProperties = changedProperties ?? throw new ArgumentNullException(nameof(changedProperties));
            UpdatedBy = updatedBy ?? throw new ArgumentNullException(nameof(updatedBy));
            UpdatedAt = OccurredAt;
        }
    }

    /// <summary>
    /// Event raised when user statistics are updated
    /// </summary>
    public class UserStatisticsUpdatedEvent : DomainEventBase
    {
        public string Username { get; }
        public string StatisticType { get; }
        public object OldValue { get; }
        public object NewValue { get; }
        public DateTime UpdatedAt { get; }

        public UserStatisticsUpdatedEvent(Guid userId, string username, string statisticType, 
            object oldValue, object newValue, int version = 1)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            StatisticType = statisticType ?? throw new ArgumentNullException(nameof(statisticType));
            OldValue = oldValue ?? throw new ArgumentNullException(nameof(oldValue));
            NewValue = newValue ?? throw new ArgumentNullException(nameof(newValue));
            UpdatedAt = OccurredAt;
        }
    }

    /// <summary>
    /// Event raised when a user achieves a new milestone or achievement
    /// </summary>
    public class UserAchievementUnlockedEvent : DomainEventBase
    {
        public string Username { get; }
        public string AchievementName { get; }
        public string AchievementDescription { get; }
        public string Category { get; }
        public DateTime UnlockedAt { get; }
        public Dictionary<string, object>? AchievementData { get; }

        public UserAchievementUnlockedEvent(Guid userId, string username, string achievementName, 
            string achievementDescription, string category, Dictionary<string, object>? achievementData = null, int version = 1)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            AchievementName = achievementName ?? throw new ArgumentNullException(nameof(achievementName));
            AchievementDescription = achievementDescription ?? throw new ArgumentNullException(nameof(achievementDescription));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            UnlockedAt = OccurredAt;
            AchievementData = achievementData;
        }
    }

    /// <summary>
    /// Event raised when user preferences are updated
    /// </summary>
    public class UserPreferencesUpdatedEvent : DomainEventBase
    {
        public string Username { get; }
        public Dictionary<string, object> OldPreferences { get; }
        public Dictionary<string, object> NewPreferences { get; }
        public DateTime UpdatedAt { get; }

        public UserPreferencesUpdatedEvent(Guid userId, string username, 
            Dictionary<string, object> oldPreferences, Dictionary<string, object> newPreferences, int version = 1)
            : base(userId, version)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            OldPreferences = oldPreferences ?? throw new ArgumentNullException(nameof(oldPreferences));
            NewPreferences = newPreferences ?? throw new ArgumentNullException(nameof(newPreferences));
            UpdatedAt = OccurredAt;
        }
    }
}
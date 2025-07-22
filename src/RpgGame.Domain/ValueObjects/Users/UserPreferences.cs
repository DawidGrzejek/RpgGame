using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgGame.Domain.Base;

namespace RpgGame.Domain.ValueObjects.Users
{
    public class UserPreferences : DomainEntity
    {
        public bool EmailNotifications { get; private set; }
        public bool GameSoundEnabled { get; private set; }
        public string Theme { get; private set; } // "dark", "light"
        public string Language { get; private set; }

        private UserPreferences() { }

        public UserPreferences(bool emailNotifications, bool gameSoundEnabled, string theme, string language)
        {
            EmailNotifications = emailNotifications;
            GameSoundEnabled = gameSoundEnabled;
            Theme = theme;
            Language = language;
        }

        public static UserPreferences Default() => new(true, true, "dark", "en");
    }
}
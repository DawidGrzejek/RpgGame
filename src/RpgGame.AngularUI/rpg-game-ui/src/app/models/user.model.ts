export interface UserProfile {
  id: string;
  username: string;
  email: string;
  roles: string[];
  createdAt: string;
  lastLoginAt?: string;
  isActive: boolean;
  statistics: UserStatistics;
  preferences: UserPreferences;
  achievements: Achievement[];
}

export interface UserStatistics {
  totalPlayTimeMinutes: number;
  charactersCreated: number;
  totalLogins: number;
  highestCharacterLevel: number;
  totalEnemiesDefeated: number;
  totalQuestsCompleted: number;
  totalDeaths: number;
  engagementLevel: string;
}

export interface UserPreferences {
  emailNotifications: boolean;
  gameSoundEnabled: boolean;
  theme: 'light' | 'dark';
  language: string;
}

export interface Achievement {
  id: string;
  name: string;
  description: string;
  category: string;
  unlockedAt: string;
  iconUrl?: string;
}

export interface UpdateProfileRequest {
  username?: string;
  email?: string;
}

export interface UpdatePreferencesRequest {
  emailNotifications?: boolean;
  gameSoundEnabled?: boolean;
  theme?: 'light' | 'dark';
  language?: string;
}

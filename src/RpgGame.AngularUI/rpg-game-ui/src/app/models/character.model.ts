export enum CharacterType {
  Warrior = 'Warrior',
  Mage = 'Mage',
  Rogue = 'Rogue'
}

export interface Character {
  id: string;
  name: string;
  health: number;
  maxHealth: number;
  level: number;
  strength: number;
  defense: number;
  characterType: CharacterType;
  experience: number;
  experienceToNextLevel: number;
  criticalChance?: number; // For Rogue
  mana?: number; // For Mage
  maxMana?: number; // For Mage
}

export interface CharacterSummary {
  id: string;
  name: string;
  level: number;
  characterType: CharacterType;
  healthPercentage: number;
}

export interface CreateCharacterRequest {
  name: string;
  type: CharacterType;
}

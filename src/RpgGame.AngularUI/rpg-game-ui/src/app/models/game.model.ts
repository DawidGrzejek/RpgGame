import { Character } from "./character.model";

export interface GameState {
  currentCharacter: Character | null | undefined; // The currently selected character, or null if none is selected
  currentLocation: GameLocation;
  gameLog: GameLogEntry[]; // Log of game events
  isInCombat: boolean; // Flag to indicate if the player is currently in combat
  currentEnemy: Enemy | null; // The enemy currently being fought, or null if not in combat
  gameStarted: boolean; // Flag to indicate if the game has started
  gameOver: boolean; // Flag to indicate if the game is over
}

export interface GameLogEntry {
  id: string; // Unique identifier for the log entry
  timestamp: Date; // Timestamp of the log entry
  message: string; // The message describing the event
  type: 'info' | 'success' | 'warning' | 'error' | 'combat' | 'system'; // Type of the log entry
  characterName?: string; // Optional character name associated with the log entry
}

export interface GameLocation {
  name: string; // Name of the location
  description: string; // Description of the location
  connectedLocations: string[]; // List of connected locations by name
  hasEnemies: boolean; // Flag to indicate if the location has enemies
  canRest: boolean; // Flag to indicate if the player can rest in this location
}

export interface Enemy {
  id: string; // Unique identifier for the enemy
  name: string; // Name of the enemy
  health: number; // Current health of the enemy
  maxHealth: number; // Maximum health of the enemy
  level: number; // Level of the enemy
  strength: number; // Strength attribute of the enemy
  defense: number; // Defense attribute of the enemy
  experienceReward: number; // Experience points rewarded for defeating this enemy
  loot?: string[]; // Optional list of loot items dropped by the enemy
  isAlive: boolean; // Flag to indicate if the enemy is currently alive
}

export interface CombatResult {
  playerDamage: number; // Damage dealt by the player
  enemyDamage: number; // Damage dealt by the enemy
  playerHealth: number; // Player's health after the combat
  enemyHealth: number; // Enemy's health after the combat
  enemyDefeated: boolean; // Flag to indicate if the enemy was defeated
  playerDefeated: boolean; // Flag to indicate if the player was defeated
  experienceGained?: number; // Optional experience gained from the combat
  loot?: string[]; // Optional loot obtained from the enemy
}

import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CharacterService } from './character.service';
import { GameApiService } from './game-api.service';
import { Enemy, GameLocation, GameLogEntry, GameState } from '../models/game.model';
import { CharacterType } from '../models/character.model';

@Injectable({
  providedIn: 'root'
})
export class TextGameEngineService {
  private gameStateSubject = new BehaviorSubject<GameState>({
    currentCharacter: null,
    currentLocation: { name: 'Town', description: 'A peaceful town', connectedLocations: ['Forest'], hasEnemies: false, canRest: true },
    gameLog: [],
    isInCombat: false,
    currentEnemy: null,
    gameStarted: false,
    gameOver: false
  });

  public gameState$ = this.gameStateSubject.asObservable();

  constructor(private characterService: CharacterService, private gameApiService: GameApiService) { }

  // =============================================================================
  // GAME INITIALIZATION
  // =============================================================================

  async startNewGame(characterName: string, characterType: CharacterType): Promise<void> {
    try {
      // Create character using API
      const character = await this.characterService.createCharacter({ name: characterName, type: characterType }).toPromise();

      const initialState: GameState = {
        ...this.gameStateSubject.value,
        currentCharacter: character,
        gameStarted: true,
        gameLog: []
      };

      this.gameStateSubject.next(initialState);

      this.addToLog(`Welcome to the realm, ${character?.name} the ${character?.characterType}!`, 'system');
      this.addToLog(`You find yourself in the town. Type 'help' to see available commands.`, 'info');
    } catch (error) {
      this.addToLog('Failed to create character. Please try again.', 'error');
    }
  }

  async loadGame(characterId: string): Promise<void> {
    try {
      const character = await this.characterService.getCharacter(characterId).toPromise();

      const loadedState: GameState = {
        ...this.gameStateSubject.value,
        currentCharacter: character,
        gameStarted: true,
        gameLog: [{
          id: this.generateId(),
          timestamp: new Date(),
          message: `Welcome back, ${character?.name}!`,
          type: 'system'
        }]
      };

      this.gameStateSubject.next(loadedState);
    } catch (error) {
      this.addToLog('Failed to load game. Please try again.', 'error');
    }
  }

  // =============================================================================
  // COMMAND PROCESSING
  // =============================================================================

  async processCommand(command: string): Promise<void> {
    const currentState = this.gameStateSubject.value;

    if (!currentState.gameStarted || !currentState.currentCharacter) {
      this.addToLog('Please start a new game first.', 'error');
      return;
    }

    const [action, ...args] = command.toLowerCase().trim().split(' ');

    this.addToLog(`> ${command}`, 'info');

    switch (action) {
      case 'help':
        this.showHelp();
        break;
      case 'stats':
      case 'status':
        this.showCharacterStats();
        break;
      case 'look':
      case 'l':
        this.lookAround();
        break;
      case 'go':
      case 'move':
      case 'travel':
        await this.travel(args.join(' '));
        break;
      case 'explore':
      case 'search':
        await this.explore();
        break;
      case 'rest':
        await this.rest();
        break;
      case 'attack':
      case 'fight':
        if (currentState.isInCombat) {
          await this.attack();
        } else {
          this.addToLog('There is nothing to attack here.', 'warning');
        }
        break;
      case 'flee':
      case 'run':
        if (currentState.isInCombat) {
          await this.flee();
        } else {
          this.addToLog('You are not in combat.', 'warning');
        }
        break;
      case 'inventory':
      case 'inv':
        await this.showInventory();
        break;
      case 'levelup':
        await this.levelUp();
        break;
      case 'save':
        await this.saveGame();
        break;
      default:
        this.addToLog(`Unknown command: ${action}. Type 'help' for available commands.`, 'warning');
    }
  }
  // =============================================================================
  // GAME ACTIONS
  // =============================================================================

  private showHelp(): void {
    const helpText = `
    === AVAILABLE COMMANDS ===
    • help - Show this help menu
    • stats/status - Show character information
    • look/l - Look around current location
    • go/move/travel [location] - Travel to another location
    • explore/search - Search for enemies or items
    • rest - Rest to recover health
    • attack/fight - Attack enemy (during combat)
    • flee/run - Flee from combat
    • inventory/inv - Show your inventory
    • levelup - Level up your character
    • save - Save your game
    ==========================`;

    this.addToLog(helpText, 'system');
  }

  private showCharacterStats(): void {
    const character = this.gameStateSubject.value.currentCharacter!;

    let statsText = `
=== CHARACTER STATS ===
Name: ${character.name}
Class: ${character.characterType}
Level: ${character.level}
Health: ${character.health}/${character.maxHealth}
Strength: ${character.strength}
Defense: ${character.defense}
Experience: ${character.experience}/${character.experienceToNextLevel}`;

    // Add class-specific stats
    if (character.characterType === 'Mage' && character.mana !== undefined) {
      statsText += `\nMana: ${character.mana}/${character.maxMana}`;
    } else if (character.characterType === 'Rogue' && character.criticalChance !== undefined) {
      statsText += `\nCritical Chance: ${(character.criticalChance * 100).toFixed(1)}%`;
    }

    statsText += '\n=======================';

    this.addToLog(statsText, 'info');
  }

  private lookAround(): void {
    const currentState = this.gameStateSubject.value;
    const location = currentState.currentLocation;

    let description = `=== ${location.name.toUpperCase()} ===\n${location.description}`;

    if (location.connectedLocations.length > 0) {
      description += `\n\nExits: ${location.connectedLocations.join(', ')}`;
    }

    if (currentState.isInCombat && currentState.currentEnemy) {
      description += `\n\n🗡️ A ${currentState.currentEnemy.name} stands before you!`;
      description += `\nEnemy Health: ${currentState.currentEnemy.health}/${currentState.currentEnemy.maxHealth}`;
    }

    description += '\n' + '='.repeat(location.name.length + 8);

    this.addToLog(description, 'info');
  }

  private async travel(destination: string): Promise<void> {
    if (!destination) {
      this.addToLog('Where do you want to go? Usage: go [location]', 'warning');
      return;
    }

    const currentState = this.gameStateSubject.value;

    if (currentState.isInCombat) {
      this.addToLog('You cannot travel while in combat! Try to flee first.', 'warning');
      return;
    }

    const currentLocation = currentState.currentLocation;
    const targetLocation = destination.toLowerCase();

    // Check if destination is accessible
    const canTravel = currentLocation.connectedLocations
      .some(loc => loc.toLowerCase() === targetLocation);

    if (!canTravel) {
      this.addToLog(`You cannot travel to ${destination} from here.`, 'warning');
      this.addToLog(`Available destinations: ${currentLocation.connectedLocations.join(', ')}`, 'info');
      return;
    }

    try {
      // Call your Web API to handle location change
      const result = await this.gameApiService.changeLocation(
        currentState.currentCharacter!.id,
        targetLocation
      ).toPromise();

      if (!result) {
        this.addToLog('Failed to travel - no response from server. Please try again.', 'error');
        return;
      }

      if (result.success) {
        const newLocation: GameLocation = {
          name: result.Location.name,
          description: result.Location.description,
          connectedLocations: result.Location.connectedLocations,
          hasEnemies: result.Location.hasEnemies,
          canRest: result.Location.canRest
        };

        const newState = {
          ...currentState,
          currentLocation: newLocation
        };

        this.gameStateSubject.next(newState);

        this.addToLog(`You travel to ${newLocation.name}.`, 'success');
        this.lookAround();

        // Random encounter check
        if (newLocation.hasEnemies && Math.random() < 0.3) {
          setTimeout(() => this.triggerRandomEncounter(), 1000);
        }
      }
    } catch (error) {
      this.addToLog('Failed to travel. Please try again.', 'error');
    }
  }

  private async explore(): Promise<void> {
    const currentState = this.gameStateSubject.value;

    if (currentState.isInCombat) {
      this.addToLog('You cannot explore while in combat!', 'warning');
      return;
    }

    this.addToLog('You search the area...', 'info');

    try {
      const result = await this.gameApiService.exploreLocation(
        currentState.currentCharacter!.id,
        currentState.currentLocation.name
      ).toPromise();

      if (!result) {
        this.addToLog('Failed to explore - no response from server. Please try again.', 'error');
        return;
      }

      if (result.enemyEncountered) {
        await this.startCombat(result.enemy);
      } else if (result.itemFound) {
        this.addToLog(`You found a ${result.itemName}!`, 'success');
      } else if (result.experienceGained > 0) {
        this.addToLog(`You gain ${result.experienceGained} experience from your exploration.`, 'success');
        await this.refreshCharacter();
      } else {
        this.addToLog('You find nothing of interest.', 'info');
      }
    } catch (error) {
      this.addToLog('Something went wrong during exploration.', 'error');
    }
  }

  private async rest(): Promise<void> {
    const currentState = this.gameStateSubject.value;

    if (currentState.isInCombat) {
      this.addToLog('You cannot rest while in combat!', 'warning');
      return;
    }

    if (!currentState.currentLocation.canRest) {
      this.addToLog('This location is not safe for resting.', 'warning');
      return;
    }

    try {
      const result = await this.gameApiService.restCharacter(
        currentState.currentCharacter!.id
      ).toPromise();

      if (!result) {
        this.addToLog('Failed to rest - no response from server. Please try again.', 'error');
        return;
      }

      if (result.success) {
        this.addToLog(`You rest and recover ${result.healthRestored} health.`, 'success');
        await this.refreshCharacter();
      }
    } catch (error) {
      this.addToLog('Failed to rest. Please try again.', 'error');
    }
  }

  // =============================================================================
  // COMBAT SYSTEM
  // =============================================================================

  private async startCombat(enemy: Enemy): Promise<void> {
    const currentState = this.gameStateSubject.value;

    const newState = {
      ...currentState,
      isInCombat: true,
      currentEnemy: enemy
    };

    this.gameStateSubject.next(newState);

    this.addToLog(`⚔️ A ${enemy.name} appears!`, 'combat');
    this.addToLog(`Enemy: ${enemy.health}/${enemy.maxHealth} HP`, 'combat');
    this.addToLog('Type "attack" to fight or "flee" to run away!', 'system');
  }

  private async attack(): Promise<void> {
    const currentState = this.gameStateSubject.value;

    if (!currentState.currentEnemy) return;

    try {
      const result = await this.gameApiService.performAttack(
        currentState.currentCharacter!.id,
        currentState.currentEnemy.id
      ).toPromise();

      if (!result) {
        this.addToLog('Attack failed - no response from server. Please try again.', 'error');
        return;
      }

      this.addToLog(`You attack the ${currentState.currentEnemy.name} for ${result.playerDamage} damage!`, 'combat');

      if (result.enemyDefeated) {
        this.addToLog(`🎉 You defeated the ${currentState.currentEnemy.name}!`, 'success');
        this.addToLog(`You gain ${result.experienceGained} experience!`, 'success');

        if (result.itemsDropped.length > 0) {
          this.addToLog(`Items dropped: ${result.itemsDropped.join(', ')}`, 'success');
        }

        // End combat
        const newState = {
          ...currentState,
          isInCombat: false,
          currentEnemy: null
        };
        this.gameStateSubject.next(newState);

        await this.refreshCharacter();
      } else {
        // Enemy attacks back
        this.addToLog(`The ${currentState.currentEnemy.name} attacks you for ${result.enemyDamage} damage!`, 'combat');

        if (result.playerDefeated) {
          await this.handlePlayerDeath();
        } else {
          // Update enemy health
          const updatedEnemy = {
            ...currentState.currentEnemy,
            health: result.enemyHealth
          };

          const newState = {
            ...currentState,
            currentEnemy: updatedEnemy
          };
          this.gameStateSubject.next(newState);

          await this.refreshCharacter();
        }
      }
    } catch (error) {
      this.addToLog('Attack failed. Please try again.', 'error');
    }
  }

  private async flee(): Promise<void> {
    const currentState = this.gameStateSubject.value;

    this.addToLog('You attempt to flee...', 'combat');

    // 80% chance to flee successfully
    if (Math.random() < 0.8) {
      const newState = {
        ...currentState,
        isInCombat: false,
        currentEnemy: null
      };
      this.gameStateSubject.next(newState);

      this.addToLog('You successfully escape from combat!', 'success');
    } else {
      this.addToLog('You failed to escape! The enemy attacks!', 'combat');
      // Enemy gets a free attack
      // This would call your API to handle enemy attack
    }
  }

  private async triggerRandomEncounter(): Promise<void> {
    this.addToLog('You hear rustling in the distance...', 'warning');

    try {
      const result = await this.gameApiService.triggerRandomEncounter(
        this.gameStateSubject.value.currentCharacter!.id,
        this.gameStateSubject.value.currentLocation.name
      ).toPromise();

      if (!result) {
        this.addToLog('Failed to trigger encounter - no response from server. Please try again.', 'error');
        return;
      }
      
      if (result.enemyEncountered) {
        await this.startCombat(result.enemy);
      }
    } catch (error) {
      this.addToLog('Something stirred in the shadows, but nothing happened.', 'info');
    }
  }

  // =============================================================================
  // CHARACTER MANAGEMENT
  // =============================================================================

  private async levelUp(): Promise<void> {
    const character = this.gameStateSubject.value.currentCharacter!;

    if (character.experience < character.experienceToNextLevel) {
      this.addToLog(`You need ${character.experienceToNextLevel - character.experience} more experience to level up.`, 'warning');
      return;
    }

    try {
      const result = await this.characterService.levelUpCharacter(character.id).toPromise();

      if (result.success) {
        this.addToLog(`🎉 ${character.name} leveled up to level ${character.level + 1}!`, 'success');
        await this.refreshCharacter();
      }
    } catch (error) {
      this.addToLog('Failed to level up. Please try again.', 'error');
    }
  }

  private async showInventory(): Promise<void> {
    const character = this.gameStateSubject.value.currentCharacter!;

    try {
      const inventory = await this.gameApiService.getInventory(character.id).toPromise();

      if (!inventory) {
        this.addToLog('Failed to load inventory - no response from server. Please try again.', 'error');
        return;
      }

      let inventoryText = `
      === INVENTORY ===
      Gold: ${inventory.gold}

      Items:`;

      if (inventory.items.length === 0) {
        inventoryText += '\n(Empty)';
      } else {
        inventory.items.forEach((item, index) => {
          inventoryText += `\n${index + 1}. ${item.name} - ${item.description}`;
        });
      }

      inventoryText += '\n================';

      this.addToLog(inventoryText, 'info');
    } catch (error) {
      this.addToLog('Failed to load inventory.', 'error');
    }
  }

  private async refreshCharacter(): Promise<void> {
    const currentState = this.gameStateSubject.value;

    try {
      const updatedCharacter = await this.characterService.getCharacter(
        currentState.currentCharacter!.id
      ).toPromise();

      const newState = {
        ...currentState,
        currentCharacter: updatedCharacter
      };

      this.gameStateSubject.next(newState);
    } catch (error) {
      console.error('Failed to refresh character:', error);
    }
  }

  private async handlePlayerDeath(): Promise<void> {
    this.addToLog('💀 You have been defeated!', 'error');
    this.addToLog('Game Over! You can start a new game or load a saved game.', 'system');

    const currentState = this.gameStateSubject.value;
    const newState = {
      ...currentState,
      gameOver: true,
      isInCombat: false,
      currentEnemy: null
    };

    this.gameStateSubject.next(newState);
  }

  // =============================================================================
  // UTILITY METHODS
  // =============================================================================

  private async saveGame(): Promise<void> {
    const character = this.gameStateSubject.value.currentCharacter!;

    try {
      const result = await this.gameApiService.saveGame(
        character.id,
        this.gameStateSubject.value.currentLocation.name
      ).toPromise();

      if (!result) {
        this.addToLog('Failed to save game - no response from server. Please try again.', 'error');
        return;
      }

      if (result.success) {
        this.addToLog('Game saved successfully!', 'success');
      }
    } catch (error) {
      this.addToLog('Failed to save game.', 'error');
    }
  }

  private addToLog(message: string, type: GameLogEntry['type'], characterName?: string): void {
    const currentState = this.gameStateSubject.value;
    const entry: GameLogEntry = {
      id: this.generateId(),
      timestamp: new Date(),
      message,
      type,
      characterName
    };

    const newState = {
      ...currentState,
      gameLog: [...currentState.gameLog, entry]
    };

    this.gameStateSubject.next(newState);
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9);
  }

  // =============================================================================
  // PUBLIC GETTERS
  // =============================================================================

  get currentGameState(): GameState {
    return this.gameStateSubject.value;
  }

  get isGameStarted(): boolean {
    return this.gameStateSubject.value.gameStarted;
  }

  get isInCombat(): boolean {
    return this.gameStateSubject.value.isInCombat;
  }
}

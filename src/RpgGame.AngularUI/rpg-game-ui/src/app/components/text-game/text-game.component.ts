// =============================================================================
// TEXT-BASED RPG GAME COMPONENT
// =============================================================================

// src/app/components/text-game/text-game.component.ts
import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { TextGameEngineService } from '../../services/text-game-engine.service';
import { GameState, GameLogEntry } from '../../models/game.model';
import { CharacterType } from '../../models/character.model';

@Component({
  selector: 'app-text-game',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './text-game.component.html',
  styleUrls: ['./text-game.component.scss']
})
export class TextGameComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('gameLog') gameLogElement!: ElementRef;
  @ViewChild('commandInput') commandInputElement!: ElementRef;

  gameState: GameState | null = null;
  currentCommand = '';
  isProcessingCommand = false;
  showNewGameDialog = false;
  showLoadGameDialog = false;

  // Character creation
  newCharacterName = '';
  selectedCharacterType: CharacterType = CharacterType.Warrior;
  characterTypes: CharacterType[] = [CharacterType.Warrior, CharacterType.Mage, CharacterType.Rogue];

  // Saved games
  savedGames: any[] = [];

  private gameStateSubscription?: Subscription;
  private shouldScrollToBottom = false;

  constructor(private gameEngine: TextGameEngineService) { }

  ngOnInit(): void {
    this.gameStateSubscription = this.gameEngine.gameState$.subscribe(
      state => {
        this.gameState = state;
        this.shouldScrollToBottom = true;
      }
    );
  }

  ngOnDestroy(): void {
    this.gameStateSubscription?.unsubscribe();
  }

  ngAfterViewChecked(): void {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  // =============================================================================
  // GAME CONTROL METHODS
  // =============================================================================

  async onSubmitCommand(): Promise<void> {
    if (!this.currentCommand.trim() || this.isProcessingCommand) {
      return;
    }

    const command = this.currentCommand.trim();
    this.currentCommand = '';
    this.isProcessingCommand = true;

    try {
      await this.gameEngine.processCommand(command);
    } catch (error) {
      console.error('Error processing command:', error);
    } finally {
      this.isProcessingCommand = false;
      // Focus back on input
      setTimeout(() => {
        this.commandInputElement?.nativeElement.focus();
      }, 100);
    }
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      this.onSubmitCommand();
    }
  }

  // =============================================================================
  // NEW GAME METHODS
  // =============================================================================

  openNewGameDialog(): void {
    this.showNewGameDialog = true;
    this.newCharacterName = '';
    this.selectedCharacterType = CharacterType.Warrior;
  }

  closeNewGameDialog(): void {
    this.showNewGameDialog = false;
  }

  async createNewGame(): Promise<void> {
    if (!this.newCharacterName.trim()) {
      return;
    }

    try {
      await this.gameEngine.startNewGame(this.newCharacterName.trim(), this.selectedCharacterType);
      this.closeNewGameDialog();

      // Focus on command input
      setTimeout(() => {
        this.commandInputElement?.nativeElement.focus();
      }, 100);
    } catch (error) {
      console.error('Failed to create new game:', error);
    }
  }

  // =============================================================================
  // LOAD GAME METHODS
  // =============================================================================

  async openLoadGameDialog(): Promise<void> {
    this.showLoadGameDialog = true;
    // Load saved games list
    // This would call your API to get saved games
    // this.savedGames = await this.gameApiService.getSavedGames().toPromise();
  }

  closeLoadGameDialog(): void {
    this.showLoadGameDialog = false;
  }

  async loadGame(characterId: string): Promise<void> {
    try {
      await this.gameEngine.loadGame(characterId);
      this.closeLoadGameDialog();

      setTimeout(() => {
        this.commandInputElement?.nativeElement.focus();
      }, 100);
    } catch (error) {
      console.error('Failed to load game:', error);
    }
  }

  // =============================================================================
  // UI HELPER METHODS
  // =============================================================================

  getLogEntryClass(entry: GameLogEntry): string {
    const baseClass = 'log-entry';
    switch (entry.type) {
      case 'success': return `${baseClass} log-success`;
      case 'error': return `${baseClass} log-error`;
      case 'warning': return `${baseClass} log-warning`;
      case 'combat': return `${baseClass} log-combat`;
      case 'system': return `${baseClass} log-system`;
      default: return `${baseClass} log-info`;
    }
  }

  getCharacterTypeDescription(type: CharacterType): string {
    switch (type) {
      case 'Warrior':
        return 'High health and strength, specializes in melee combat';
      case 'Mage':
        return 'Lower health but powerful magical abilities and mana';
      case 'Rogue':
        return 'Balanced stats with critical strike abilities';
      default:
        return '';
    }
  }

  formatTimestamp(timestamp: Date): string {
    return new Date(timestamp).toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  }

  private scrollToBottom(): void {
    if (this.gameLogElement) {
      const element = this.gameLogElement.nativeElement;
      element.scrollTop = element.scrollHeight;
    }
  }

  // =============================================================================
  // QUICK ACTION METHODS (For mobile/easier interaction)
  // =============================================================================

  quickCommand(command: string): void {
    this.currentCommand = command;
    this.onSubmitCommand();
  }

  getQuickActions(): string[] {
    if (!this.gameState) return [];

    const baseActions = ['look', 'stats', 'inventory'];

    if (this.gameState.isInCombat) {
      return [...baseActions, 'attack', 'flee'];
    } else {
      const actions = [...baseActions, 'explore'];

      if (this.gameState.currentLocation.canRest) {
        actions.push('rest');
      }

      if (this.gameState.currentLocation.connectedLocations.length > 0) {
        actions.push('go');
      }

      return actions;
    }
  }

  getHealthPercentage(): number {
    if (!this.gameState?.currentCharacter) return 0;

    const char = this.gameState.currentCharacter;
    return Math.round((char.health / char.maxHealth) * 100);
  }

  getExperiencePercentage(): number {
    if (!this.gameState?.currentCharacter) return 0;

    const char = this.gameState.currentCharacter;
    return Math.round((char.experience / char.experienceToNextLevel) * 100);
  }
}

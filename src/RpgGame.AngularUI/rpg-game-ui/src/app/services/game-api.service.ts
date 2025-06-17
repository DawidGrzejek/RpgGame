// =============================================================================
// GAME API SERVICE
// =============================================================================

// src/app/services/game-api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of } from 'rxjs';
import { environment } from '../../environments/environment';

// API Response interfaces
interface LocationChangeResponse {
  success: boolean;
  Location: {
    name: string;
    description: string;
    connectedLocations: string[];
    hasEnemies: boolean;
    canRest: boolean;
  };
  message?: string;
}

interface ExploreResponse {
  success: boolean;
  enemyEncountered: boolean;
  enemy?: any;
  itemFound: boolean;
  itemName?: string;
  experienceGained: number;
  message: string;
}

interface CombatResponse {
  playerDamage: number;
  enemyDamage: number;
  playerHealth: number;
  enemyHealth: number;
  enemyDefeated: boolean;
  playerDefeated: boolean;
  experienceGained: number;
  itemsDropped: string[];
}

interface RestResponse {
  success: boolean;
  healthRestored: number;
  message: string;
}

interface SaveGameResponse {
  success: boolean;
  message: string;
}

interface InventoryResponse {
  items: Array<{
    id: string;
    name: string;
    description: string;
    type: string;
    value: number;
  }>;
  gold: number;
  capacity: number;
}

@Injectable({
  providedIn: 'root'
})
export class GameApiService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // =============================================================================
  // LOCATION AND EXPLORATION
  // =============================================================================

  changeLocation(characterId: string, locationName: string): Observable<LocationChangeResponse | undefined> {
    return this.http.post<LocationChangeResponse>(`${this.apiUrl}/v1/game/move`, {
      characterId,
      locationName
    }).pipe(
      catchError((error) => {
        console.error('Change location error:', error);
        return of(undefined);
      })
    );
  }

  exploreLocation(characterId: string, locationName: string): Observable<ExploreResponse | undefined> {
    return this.http.post<ExploreResponse>(`${this.apiUrl}/v1/game/explore`, {
      characterId,
      locationName
    }).pipe(
      catchError((error) => {
        console.error('Explore location error:', error);
        return of(undefined);
      })
    );
  }

  triggerRandomEncounter(characterId: string, locationName: string): Observable<ExploreResponse | undefined> {
    return this.http.post<ExploreResponse>(`${this.apiUrl}/v1/game/random-encounter`, {
      characterId,
      locationName
    }).pipe(
      catchError((error) => {
        console.error('Random encounter error:', error);
        return of(undefined);
      })
    );
  }

  performAttack(characterId: string, enemyId: string): Observable<CombatResponse | undefined> {
    return this.http.post<CombatResponse>(`${this.apiUrl}/v1/combat/attack`, {
      attackerId: characterId,
      defenderId: enemyId
    }).pipe(
      catchError((error) => {
        console.error('Perform attack error:', error);
        return of(undefined);
      })
    );
  }

  fleeCombat(characterId: string, enemyId: string): Observable<{ success: boolean; message: string; enemyAttack?: CombatResponse } | undefined> {
    return this.http.post<any>(`${this.apiUrl}/v1/combat/flee`, {
      characterId,
      enemyId
    }).pipe(
      catchError((error) => {
        console.error('Flee combat error:', error);
        return of(undefined);
      })
    );
  }

  // =============================================================================
  // CHARACTER ACTIONS - Updated return types
  // =============================================================================

  restCharacter(characterId: string): Observable<RestResponse | undefined> {
    return this.http.post<RestResponse>(`${this.apiUrl}/v1/characters/${characterId}/rest`, {}).pipe(
      catchError((error) => {
        console.error('Rest character error:', error);
        return of(undefined);
      })
    );
  }

  getInventory(characterId: string): Observable<InventoryResponse> {
    return this.http.get<InventoryResponse>(`${this.apiUrl}/v1/characters/${characterId}/inventory`);
  }

  useItem(characterId: string, itemId: string): Observable<{ success: boolean; message: string; effect?: any }> {
    return this.http.post<any>(`${this.apiUrl}/v1/characters/${characterId}/inventory/use/${itemId}`, {});
  }

  equipItem(characterId: string, itemId: string): Observable<{ success: boolean; message: string }> {
    return this.http.post<any>(`${this.apiUrl}/v1/characters/${characterId}/inventory/equip/${itemId}`, {});
  }

  // =============================================================================
  // GAME STATE MANAGEMENT
  // =============================================================================

  saveGame(characterId: string, locationName: string): Observable<SaveGameResponse> {
    return this.http.post<SaveGameResponse>(`${this.apiUrl}/v1/game/save`, {
      characterId,
      locationName,
      saveName: `AutoSave_${new Date().getTime()}`
    });
  }

  getSavedGames(): Observable<Array<{
    saveName: string;
    characterId: string;
    characterName: string;
    level: number;
    characterType: string;
    saveDate: string;
    locationName: string;
  }>> {
    return this.http.get<any>(`${this.apiUrl}/v1/saves`);
  }

  loadGame(saveName: string): Observable<{
    success: boolean;
    character: any;
    location: any;
    message?: string;
  }> {
    return this.http.get<any>(`${this.apiUrl}/v1/saves/${saveName}`);
  }
}

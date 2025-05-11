import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Character, CharacterSummary, CreateCharacterRequest } from '../models/character.model';


@Injectable({
  providedIn: 'root'
})
export class CharacterService {
  private apiUrl = 'httpS://localhost:7153/api/v1/characters';

  constructor(private http: HttpClient) { }

  getCharacters(): Observable<CharacterSummary[]> {
    return this.http.get<CharacterSummary[]>(this.apiUrl);
  }

  getCharacter(id: string): Observable<Character> {
    return this.http.get<Character>(`${this.apiUrl}/${id}`);
  }

  createCharacter(character: CreateCharacterRequest): Observable<Character> {
    return this.http.post<Character>(this.apiUrl, character);
  }

  levelUpCharacter(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/levelup`, {});
  }
}

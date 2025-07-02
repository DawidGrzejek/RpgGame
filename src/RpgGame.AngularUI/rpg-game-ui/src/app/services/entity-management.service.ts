import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EnemyTemplate, ItemTemplate } from '../models/entity-template.model';

@Injectable({
  providedIn: 'root'
})
export class EntityManagementService {
  private apiUrl = 'https://localhost:7153/api/v1'; // Replace with your actual API URL

  constructor(private http: HttpClient) {}

  // Enemy Template Methods
  getEnemyTemplates(): Observable<EnemyTemplate[]> {
    return this.http.get<EnemyTemplate[]>(`${this.apiUrl}/enemy-templates`);
  }

  getEnemyTemplate(id: string): Observable<EnemyTemplate> {
    return this.http.get<EnemyTemplate>(`${this.apiUrl}/enemy-templates/${id}`);
  }

  createEnemyTemplate(template: Partial<EnemyTemplate>): Observable<EnemyTemplate> {
    return this.http.post<EnemyTemplate>(`${this.apiUrl}/enemy-templates`, template);
  }

  updateEnemyTemplate(id: string, template: Partial<EnemyTemplate>): Observable<EnemyTemplate> {
    return this.http.put<EnemyTemplate>(`${this.apiUrl}/enemy-templates/${id}`, template);
  }

  deleteEnemyTemplate(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/enemy-templates/${id}`);
  }

  // Item Template Methods
  getItemTemplates(): Observable<ItemTemplate[]> {
    return this.http.get<ItemTemplate[]>(`${this.apiUrl}/item-templates`);
  }

  getItemTemplate(id: string): Observable<ItemTemplate> {
    return this.http.get<ItemTemplate>(`${this.apiUrl}/item-templates/${id}`);
  }

  createItemTemplate(template: Partial<ItemTemplate>): Observable<ItemTemplate> {
    return this.http.post<ItemTemplate>(`${this.apiUrl}/item-templates`, template);
  }

  updateItemTemplate(id: string, template: Partial<ItemTemplate>): Observable<ItemTemplate> {
    return this.http.put<ItemTemplate>(`${this.apiUrl}/item-templates/${id}`, template);
  }

  deleteItemTemplate(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/item-templates/${id}`);
  }
}

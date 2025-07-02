import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EntityManagementService } from '../../services/entity-management.service';
import { EnemyTemplate, ItemTemplate } from '../../models/entity-template.model';

interface EntityHierarchy {
  enemies: {
    [key: string]: EnemyTemplate[]
  };
  items: {
    [key: string]: ItemTemplate[]
  };
}
@Component({
  selector: 'app-entity-hierarchy',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="entity-hierarchy">
      <div class="hierarchy-header">
        <h1>Game Entity Hierarchy</h1>
        <div class="actions">
          <button (click)="createNewEnemy()" class="btn btn-primary">Create Enemy</button>
          <button (click)="createNewItem()" class="btn btn-primary">Create Item</button>
        </div>
      </div>

      <div class="hierarchy-content">
        <!-- Enemy Hierarchy -->
        <div class="entity-section">
          <h2>Enemies</h2>
          <div class="entity-tree">
            <div *ngFor="let enemyType of enemyTypes" class="entity-group">
              <h3 class="entity-type-header">
                <i class="icon-enemy-{{enemyType.toLowerCase()}}"></i>
                {{enemyType}} ({{hierarchy.enemies[enemyType]?.length || 0}})
              </h3>
              <div class="entity-list">
                <div *ngFor="let enemy of hierarchy.enemies[enemyType]"
                     class="entity-item"
                     (click)="editEnemy(enemy)">
                  <div class="entity-info">
                    <span class="entity-name">{{enemy.name}}</span>
                    <span class="entity-stats">
                      HP: {{enemy.baseHealth}} |
                      STR: {{enemy.baseStrength}} |
                      DEF: {{enemy.baseDefense}} |
                      EXP: {{enemy.experienceReward}}
                    </span>
                  </div>
                  <div class="entity-actions">
                    <button (click)="duplicateEnemy(enemy); $event.stopPropagation()"
                            class="btn btn-sm">Duplicate</button>
                    <button (click)="deleteEnemy(enemy); $event.stopPropagation()"
                            class="btn btn-danger btn-sm">Delete</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Item Hierarchy -->
        <div class="entity-section">
          <h2>Items</h2>
          <div class="entity-tree">
            <div *ngFor="let itemType of itemTypes" class="entity-group">
              <h3 class="entity-type-header">
                <i class="icon-item-{{itemType.toLowerCase()}}"></i>
                {{itemType}} ({{hierarchy.items[itemType]?.length || 0}})
              </h3>
              <div class="entity-list">
                <div *ngFor="let item of hierarchy.items[itemType]"
                     class="entity-item"
                     (click)="editItem(item)">
                  <div class="entity-info">
                    <span class="entity-name">{{item.name}}</span>
                    <span class="entity-stats">
                      Value: {{item.value}} |
                      <span *ngIf="item.isEquippable">Equippable</span>
                      <span *ngIf="item.isConsumable">Consumable</span>
                    </span>
                  </div>
                  <div class="entity-actions">
                    <button (click)="duplicateItem(item); $event.stopPropagation()"
                            class="btn btn-sm">Duplicate</button>
                    <button (click)="deleteItem(item); $event.stopPropagation()"
                            class="btn btn-danger btn-sm">Delete</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./entity-hierarchy.component.scss']
})


export class EntityHierarchyComponent implements OnInit {
  hierarchy: EntityHierarchy = {
    enemies: {},
    items: {}
  };

  enemyTypes = ['Beast', 'Undead', 'Dragon', 'Humanoid'];
  itemTypes = ['Weapon', 'Armor', 'Consumable', 'Misc'];

  constructor(private entityService: EntityManagementService) {}

  ngOnInit(): void {
    this.loadHierarchy();
  }

  loadHierarchy(): void {
    // Load enemies
    this.entityService.getEnemyTemplates().subscribe(enemies => {
      this.hierarchy.enemies = this.groupByType(enemies, 'enemyType');
    });

    // Load items
    this.entityService.getItemTemplates().subscribe(items => {
      this.hierarchy.items = this.groupByType(items, 'itemType');
    });
  }

  private groupByType<T>(items: T[], typeProperty: keyof T): { [key: string]: T[] } {
    return items.reduce((groups, item) => {
      const type = String(item[typeProperty]);
      if (!groups[type]) {
        groups[type] = [];
      }
      groups[type].push(item);
      return groups;
    }, {} as { [key: string]: T[] });
  }

  createNewEnemy(): void {
    // Navigate to enemy creation form
  }

  createNewItem(): void {
    // Navigate to item creation form
  }

  editEnemy(enemy: EnemyTemplate): void {
    // Navigate to enemy edit form
  }

  editItem(item: ItemTemplate): void {
    // Navigate to item edit form
  }

  duplicateEnemy(enemy: EnemyTemplate): void {
    const duplicate = { ...enemy, id: '', name: `${enemy.name} (Copy)` };
    this.entityService.createEnemyTemplate(duplicate).subscribe(() => {
      this.loadHierarchy();
    });
  }

  duplicateItem(item: ItemTemplate): void {
    const duplicate = { ...item, id: '', name: `${item.name} (Copy)` };
    this.entityService.createItemTemplate(duplicate).subscribe(() => {
      this.loadHierarchy();
    });
  }

  deleteEnemy(enemy: EnemyTemplate): void {
    if (confirm(`Are you sure you want to delete ${enemy.name}?`)) {
      this.entityService.deleteEnemyTemplate(enemy.id).subscribe(() => {
        this.loadHierarchy();
      });
    }
  }

  deleteItem(item: ItemTemplate): void {
    if (confirm(`Are you sure you want to delete ${item.name}?`)) {
      this.entityService.deleteItemTemplate(item.id).subscribe(() => {
        this.loadHierarchy();
      });
    }
  }
}

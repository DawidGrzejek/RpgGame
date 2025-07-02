import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { EntityManagementService } from '../../services/entity-management.service';
import { EnemyTemplate, EnemyType } from '../../models/entity-template.model';

@Component({
  selector: 'app-enemy-creation',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="entity-creation-container">
      <div class="creation-header">
        <h1>{{ isEditMode ? 'Edit Enemy' : 'Create New Enemy' }}</h1>
        <button type="button" (click)="goBack()" class="btn btn-outline">Cancel</button>
      </div>

      <form [formGroup]="enemyForm" (ngSubmit)="onSubmit()" class="entity-form">
        <div class="form-section">
          <h2>Basic Information</h2>

          <div class="form-group">
            <label for="name">Name *</label>
            <input
              type="text"
              id="name"
              formControlName="name"
              class="form-control"
              placeholder="Enter enemy name">
            <div *ngIf="enemyForm.get('name')?.invalid && enemyForm.get('name')?.touched" class="error">
              Name is required and must be at least 3 characters long.
            </div>
          </div>

          <div class="form-group">
            <label for="description">Description</label>
            <textarea
              id="description"
              formControlName="description"
              class="form-control"
              rows="3"
              placeholder="Describe this enemy..."></textarea>
          </div>

          <div class="form-group">
            <label for="enemyType">Enemy Type *</label>
            <select id="enemyType" formControlName="enemyType" class="form-control">
              <option value="">Select enemy type</option>
              <option *ngFor="let type of enemyTypes" [value]="type">{{ type }}</option>
            </select>
            <div *ngIf="enemyForm.get('enemyType')?.invalid && enemyForm.get('enemyType')?.touched" class="error">
              Enemy type is required.
            </div>
          </div>
        </div>

        <div class="form-section">
          <h2>Combat Stats</h2>

          <div class="stats-grid">
            <div class="form-group">
              <label for="baseHealth">Health *</label>
              <input
                type="number"
                id="baseHealth"
                formControlName="baseHealth"
                class="form-control"
                min="1"
                placeholder="100">
              <div *ngIf="enemyForm.get('baseHealth')?.invalid && enemyForm.get('baseHealth')?.touched" class="error">
                Health must be at least 1.
              </div>
            </div>

            <div class="form-group">
              <label for="baseStrength">Strength *</label>
              <input
                type="number"
                id="baseStrength"
                formControlName="baseStrength"
                class="form-control"
                min="1"
                placeholder="10">
              <div *ngIf="enemyForm.get('baseStrength')?.invalid && enemyForm.get('baseStrength')?.touched" class="error">
                Strength must be at least 1.
              </div>
            </div>

            <div class="form-group">
              <label for="baseDefense">Defense *</label>
              <input
                type="number"
                id="baseDefense"
                formControlName="baseDefense"
                class="form-control"
                min="0"
                placeholder="5">
              <div *ngIf="enemyForm.get('baseDefense')?.invalid && enemyForm.get('baseDefense')?.touched" class="error">
                Defense cannot be negative.
              </div>
            </div>

            <div class="form-group">
              <label for="experienceReward">Experience Reward *</label>
              <input
                type="number"
                id="experienceReward"
                formControlName="experienceReward"
                class="form-control"
                min="1"
                placeholder="50">
              <div *ngIf="enemyForm.get('experienceReward')?.invalid && enemyForm.get('experienceReward')?.touched" class="error">
                Experience reward must be at least 1.
              </div>
            </div>
          </div>
        </div>

        <div class="form-section">
          <h2>Loot Table</h2>
          <p class="section-description">Items this enemy can drop when defeated</p>

          <div formArrayName="possibleLoot" class="loot-list">
            <div *ngFor="let loot of lootArray.controls; let i = index" class="loot-item">
              <input
                type="text"
                [formControlName]="i"
                class="form-control"
                placeholder="Item name">
              <button
                type="button"
                (click)="removeLootItem(i)"
                class="btn btn-danger btn-sm">Remove</button>
            </div>
          </div>

          <button type="button" (click)="addLootItem()" class="btn btn-outline">Add Loot Item</button>
        </div>

        <div class="form-section">
          <h2>Special Abilities</h2>
          <p class="section-description">Configure special abilities and their parameters</p>

          <div class="abilities-list">
            <div *ngFor="let ability of specialAbilities; let i = index" class="ability-item">
              <div class="ability-header">
                <input
                  type="text"
                  [(ngModel)]="ability.name"
                  [ngModelOptions]="{standalone: true}"
                  class="form-control ability-name"
                  placeholder="Ability name">
                <button
                  type="button"
                  (click)="removeAbility(i)"
                  class="btn btn-danger btn-sm">Remove</button>
              </div>

              <div class="ability-config">
                <textarea
                  [(ngModel)]="ability.config"
                  [ngModelOptions]="{standalone: true}"
                  class="form-control"
                  rows="2"
                  placeholder="Ability configuration (JSON format)"></textarea>
              </div>
            </div>
          </div>

          <button type="button" (click)="addAbility()" class="btn btn-outline">Add Special Ability</button>
        </div>

        <div class="form-actions">
          <button type="button" (click)="goBack()" class="btn btn-outline">Cancel</button>
          <button type="submit" [disabled]="enemyForm.invalid || submitting" class="btn btn-primary">
            {{ submitting ? 'Saving...' : (isEditMode ? 'Update Enemy' : 'Create Enemy') }}
          </button>
        </div>

        <div *ngIf="error" class="error-message">
          {{ error }}
        </div>
      </form>
    </div>
  `,
  styleUrls: ['./enemy-creation.component.scss']
})
export class EnemyCreationComponent implements OnInit {
  enemyForm: FormGroup;
  enemyTypes = Object.values(EnemyType);
  specialAbilities: { name: string; config: string }[] = [];
  isEditMode = false;
  submitting = false;
  error = '';
  enemyId?: string;

  constructor(
    private fb: FormBuilder,
    private entityService: EntityManagementService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.enemyForm = this.createForm();
  }

  ngOnInit(): void {
    this.enemyId = this.route.snapshot.paramMap.get('id') || undefined;
    this.isEditMode = !!this.enemyId;

    if (this.isEditMode && this.enemyId) {
      this.loadEnemy(this.enemyId);
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      enemyType: ['', Validators.required],
      baseHealth: [100, [Validators.required, Validators.min(1)]],
      baseStrength: [10, [Validators.required, Validators.min(1)]],
      baseDefense: [5, [Validators.required, Validators.min(0)]],
      experienceReward: [50, [Validators.required, Validators.min(1)]],
      possibleLoot: this.fb.array([])
    });
  }

  get lootArray(): FormArray {
    return this.enemyForm.get('possibleLoot') as FormArray;
  }

  private loadEnemy(id: string): void {
    this.entityService.getEnemyTemplate(id).subscribe({
      next: (enemy) => {
        this.enemyForm.patchValue({
          name: enemy.name,
          description: enemy.description,
          enemyType: enemy.enemyType,
          baseHealth: enemy.baseHealth,
          baseStrength: enemy.baseStrength,
          baseDefense: enemy.baseDefense,
          experienceReward: enemy.experienceReward
        });

        // Load loot items
        this.lootArray.clear();
        if (enemy.possibleLoot) {
          enemy.possibleLoot.forEach(loot => {
            this.lootArray.push(this.fb.control(loot));
          });
        }

        // Load special abilities
        this.specialAbilities = Object.entries(enemy.specialAbilities).map(([name, config]) => ({
          name,
          config: JSON.stringify(config, null, 2)
        }));
      },
      error: (err) => {
        this.error = 'Failed to load enemy data';
        console.error(err);
      }
    });
  }

  addLootItem(): void {
    this.lootArray.push(this.fb.control(''));
  }

  removeLootItem(index: number): void {
    this.lootArray.removeAt(index);
  }

  addAbility(): void {
    this.specialAbilities.push({ name: '', config: '{}' });
  }

  removeAbility(index: number): void {
    this.specialAbilities.splice(index, 1);
  }

  onSubmit(): void {
    if (this.enemyForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.submitting = true;
    this.error = '';

    const formValue = this.enemyForm.value;

    // Parse special abilities
    const specialAbilities: { [key: string]: any } = {};
    this.specialAbilities.forEach(ability => {
      if (ability.name.trim()) {
        try {
          specialAbilities[ability.name] = JSON.parse(ability.config || '{}');
        } catch (e) {
          specialAbilities[ability.name] = ability.config;
        }
      }
    });

    const enemyData = {
      ...formValue,
      possibleLoot: formValue.possibleLoot.filter((loot: string) => loot.trim()),
      specialAbilities
    };

    const operation = this.isEditMode
      ? this.entityService.updateEnemyTemplate(this.enemyId!, enemyData)
      : this.entityService.createEnemyTemplate(enemyData);

    operation.subscribe({
      next: () => {
        this.submitting = false;
        this.router.navigate(['/admin/entities']);
      },
      error: (err) => {
        this.submitting = false;
        this.error = err.error?.message || 'Failed to save enemy';
        console.error(err);
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.enemyForm.controls).forEach(key => {
      const control = this.enemyForm.get(key);
      control?.markAsTouched();
    });
  }

  goBack(): void {
    this.router.navigate(['/admin/entities']);
  }
}

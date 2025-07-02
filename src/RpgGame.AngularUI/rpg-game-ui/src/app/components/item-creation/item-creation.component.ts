import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { EntityManagementService } from '../../services/entity-management.service';
import { ItemTemplate, ItemType, EquipmentSlot } from '../../models/entity-template.model';

@Component({
  selector: 'app-item-creation',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="entity-creation-container">
      <div class="creation-header">
        <h1>{{ isEditMode ? 'Edit Item' : 'Create New Item' }}</h1>
        <button type="button" (click)="goBack()" class="btn btn-outline">Cancel</button>
      </div>

      <form [formGroup]="itemForm" (ngSubmit)="onSubmit()" class="entity-form">
        <div class="form-section">
          <h2>Basic Information</h2>

          <div class="form-group">
            <label for="name">Name *</label>
            <input
              type="text"
              id="name"
              formControlName="name"
              class="form-control"
              placeholder="Enter item name">
            <div *ngIf="itemForm.get('name')?.invalid && itemForm.get('name')?.touched" class="error">
              Name is required and must be at least 2 characters long.
            </div>
          </div>

          <div class="form-group">
            <label for="description">Description</label>
            <textarea
              id="description"
              formControlName="description"
              class="form-control"
              rows="3"
              placeholder="Describe this item..."></textarea>
          </div>

          <div class="form-group">
            <label for="itemType">Item Type *</label>
            <select id="itemType" formControlName="itemType" class="form-control">
              <option value="">Select item type</option>
              <option *ngFor="let type of itemTypes" [value]="type">{{ type }}</option>
            </select>
            <div *ngIf="itemForm.get('itemType')?.invalid && itemForm.get('itemType')?.touched" class="error">
              Item type is required.
            </div>
          </div>

          <div class="form-group">
            <label for="value">Value *</label>
            <input
              type="number"
              id="value"
              formControlName="value"
              class="form-control"
              min="0"
              placeholder="100">
            <div *ngIf="itemForm.get('value')?.invalid && itemForm.get('value')?.touched" class="error">
              Value cannot be negative.
            </div>
          </div>
        </div>

        <div class="form-section">
          <h2>Item Properties</h2>

          <div class="checkbox-group">
            <label class="checkbox-label">
              <input type="checkbox" formControlName="isConsumable">
              <span>Consumable Item</span>
              <small>Item is consumed when used</small>
            </label>

            <label class="checkbox-label">
              <input type="checkbox" formControlName="isEquippable">
              <span>Equippable Item</span>
              <small>Item can be equipped by characters</small>
            </label>
          </div>

          <div *ngIf="itemForm.get('isEquippable')?.value" class="form-group">
            <label for="equipmentSlot">Equipment Slot</label>
            <select id="equipmentSlot" formControlName="equipmentSlot" class="form-control">
              <option value="">Select equipment slot</option>
              <option *ngFor="let slot of equipmentSlots" [value]="slot">{{ slot }}</option>
            </select>
          </div>
        </div>

        <div class="form-section">
          <h2>Stat Modifiers</h2>
          <p class="section-description">Bonuses this item provides when equipped</p>

          <div class="stat-modifiers">
            <div *ngFor="let modifier of statModifiers; let i = index" class="modifier-item">
              <input
                type="text"
                [(ngModel)]="modifier.stat"
                [ngModelOptions]="{standalone: true}"
                class="form-control modifier-stat"
                placeholder="Stat name (e.g., Strength)">
              <input
                type="number"
                [(ngModel)]="modifier.value"
                [ngModelOptions]="{standalone: true}"
                class="form-control modifier-value"
                placeholder="Modifier value">
              <button
                type="button"
                (click)="removeStatModifier(i)"
                class="btn btn-danger btn-sm">Remove</button>
            </div>
          </div>

          <button type="button" (click)="addStatModifier()" class="btn btn-outline">Add Stat Modifier</button>
        </div>

        <div class="form-actions">
          <button type="button" (click)="goBack()" class="btn btn-outline">Cancel</button>
          <button type="submit" [disabled]="itemForm.invalid || submitting" class="btn btn-primary">
            {{ submitting ? 'Saving...' : (isEditMode ? 'Update Item' : 'Create Item') }}
          </button>
        </div>

        <div *ngIf="error" class="error-message">
          {{ error }}
        </div>
      </form>
    </div>
  `,
  styleUrls: ['./item-creation.component.scss']
})
export class ItemCreationComponent implements OnInit {
  itemForm: FormGroup;
  itemTypes = Object.values(ItemType);
  equipmentSlots = Object.values(EquipmentSlot);
  statModifiers: { stat: string; value: number }[] = [];
  isEditMode = false;
  submitting = false;
  error = '';
  itemId?: string;

  constructor(
    private fb: FormBuilder,
    private entityService: EntityManagementService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.itemForm = this.createForm();
  }

  ngOnInit(): void {
    this.itemId = this.route.snapshot.paramMap.get('id') || undefined;
    this.isEditMode = !!this.itemId;

    if (this.isEditMode && this.itemId) {
      this.loadItem(this.itemId);
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      itemType: ['', Validators.required],
      value: [0, [Validators.required, Validators.min(0)]],
      isConsumable: [false],
      isEquippable: [false],
      equipmentSlot: ['']
    });
  }

  private loadItem(id: string): void {
    this.entityService.getItemTemplate(id).subscribe({
      next: (item) => {
        this.itemForm.patchValue({
          name: item.name,
          description: item.description,
          itemType: item.itemType,
          value: item.value,
          isConsumable: item.isConsumable,
          isEquippable: item.isEquippable,
          equipmentSlot: item.equipmentSlot || ''
        });

        // Load stat modifiers
        this.statModifiers = Object.entries(item.statModifiers).map(([stat, value]) => ({
          stat,
          value
        }));
      },
      error: (err) => {
        this.error = 'Failed to load item data';
        console.error(err);
      }
    });
  }

  addStatModifier(): void {
    this.statModifiers.push({ stat: '', value: 0 });
  }

  removeStatModifier(index: number): void {
    this.statModifiers.splice(index, 1);
  }

  onSubmit(): void {
    if (this.itemForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.submitting = true;
    this.error = '';

    const formValue = this.itemForm.value;

    // Convert stat modifiers to dictionary
    const statModifiers: { [key: string]: number } = {};
    this.statModifiers.forEach(modifier => {
      if (modifier.stat.trim()) {
        statModifiers[modifier.stat] = modifier.value;
      }
    });

    const itemData = {
      ...formValue,
      statModifiers,
      equipmentSlot: formValue.isEquippable ? formValue.equipmentSlot : null
    };

    const operation = this.isEditMode
      ? this.entityService.updateItemTemplate(this.itemId!, itemData)
      : this.entityService.createItemTemplate(itemData);

    operation.subscribe({
      next: () => {
        this.submitting = false;
        this.router.navigate(['/admin/entities']);
      },
      error: (err) => {
        this.submitting = false;
        this.error = err.error?.message || 'Failed to save item';
        console.error(err);
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.itemForm.controls).forEach(key => {
      const control = this.itemForm.get(key);
      control?.markAsTouched();
    });
  }

  goBack(): void {
    this.router.navigate(['/admin/entities']);
  }
}

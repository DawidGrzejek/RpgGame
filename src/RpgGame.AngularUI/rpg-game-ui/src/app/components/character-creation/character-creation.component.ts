import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { CharacterService } from '../../services/character.service';
import { CharacterType } from '../../models/character.model';

@Component({
  selector: 'app-character-creation',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './character-creation.component.html',
  styleUrls: ['./character-creation.component.scss']
})
export class CharacterCreationComponent {
  characterForm: FormGroup;
  characterTypes = Object.values(CharacterType);
  submitting = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private characterService: CharacterService,
    private router: Router
  ) {
    this.characterForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(20)]],
      type: [CharacterType.Warrior, Validators.required]
    });
  }

  onSubmit(): void {
    if (this.characterForm.invalid) {
      return;
    }

    this.submitting = true;
    this.error = '';

    this.characterService.createCharacter(this.characterForm.value)
      .subscribe({
        next: (character) => {
          this.submitting = false;
          this.router.navigate(['/characters', character.id]);
        },
        error: (err) => {
          this.submitting = false;
          this.error = err.error?.message || 'Failed to create character';
          console.error(err);
        }
      });
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CharacterService } from '../../services/character.service';
import { Character } from '../../models/character.model';

@Component({
  selector: 'app-character-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './character-detail.component.html',
  styleUrls: ['./character-detail.component.scss']
})
export class CharacterDetailComponent implements OnInit {
  character: Character | null = null;
  loading = true;
  error = '';
  levelingUp = false;

  constructor(
    private route: ActivatedRoute,
    private characterService: CharacterService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadCharacter(id);
    } else {
      this.error = 'Character ID not provided';
      this.loading = false;
    }
  }

  loadCharacter(id: string): void {
    this.loading = true;
    this.characterService.getCharacter(id)
      .subscribe({
        next: (data) => {
          this.character = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load character';
          this.loading = false;
          console.error(err);
        }
      });
  }

  onLevelUp(): void {
    if (!this.character) return;

    this.levelingUp = true;
    this.characterService.levelUpCharacter(this.character.id)
      .subscribe({
        next: () => {
          this.levelingUp = false;
          // Reload character data to show updated stats
          this.loadCharacter(this.character!.id);
        },
        error: (err) => {
          this.levelingUp = false;
          this.error = 'Failed to level up character';
          console.error(err);
        }
      });
  }
}

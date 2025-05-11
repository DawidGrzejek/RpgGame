import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CharacterService } from '../../services/character.service';
import { CharacterSummary } from '../../models/character.model';

@Component({
  selector: 'app-character-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './character-list.component.html',
  styleUrls: ['./character-list.component.scss']
})
export class CharacterListComponent implements OnInit {
  characters: CharacterSummary[] = [];
  loading = true;
  error = '';

  constructor(private characterService: CharacterService) { }

  ngOnInit(): void {
    this.loadCharacters();
  }

  loadCharacters(): void {
    this.loading = true;
    this.characterService.getCharacters()
      .subscribe({
        next: (data) => {
          this.characters = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load characters';
          this.loading = false;
          console.error(err);
        }
      });
  }
}

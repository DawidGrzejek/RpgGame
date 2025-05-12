import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CharacterService } from '../../services/character.service';
import { CharacterSummary } from '../../models/character.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  recentCharacters: CharacterSummary[] = [];
  loading = false;
  error = '';

  constructor(private characterService: CharacterService) { }

  ngOnInit(): void {
    this.loadRecentCharacters();
  }

  loadRecentCharacters(): void {
    this.loading = true;
    this.characterService.getCharacters()
      .subscribe({
        next: (characters) => {
          // Get most recent characters (up to 3)
          this.recentCharacters = characters.slice(0, 3);
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

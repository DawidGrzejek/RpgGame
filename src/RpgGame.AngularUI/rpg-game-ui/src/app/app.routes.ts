import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { CharacterListComponent } from './components/character-list/character-list.component';
import { CharacterDetailComponent } from './components/character-detail/character-detail.component';
import { CharacterCreationComponent } from './components/character-creation/character-creation.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'characters', component: CharacterListComponent },
  { path: 'characters/new', component: CharacterCreationComponent },
  { path: 'characters/:id', component: CharacterDetailComponent }
];

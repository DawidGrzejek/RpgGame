import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { CharacterListComponent } from './components/character-list/character-list.component';
import { CharacterDetailComponent } from './components/character-detail/character-detail.component';
import { CharacterCreationComponent } from './components/character-creation/character-creation.component';
import { EntityHierarchyComponent } from './components/entity-hierarchy/entity-hierarchy.component';
import { EnemyCreationComponent } from './components/enemy-creation/enemy-creation.component';
import { ItemCreationComponent } from './components/item-creation/item-creation.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'characters', component: CharacterListComponent },
  { path: 'characters/new', component: CharacterCreationComponent },
  { path: 'characters/:id', component: CharacterDetailComponent },

  // Admin routes for entity management
  {
    path: 'admin',
    children: [
      { path: 'entities', component: EntityHierarchyComponent },
      { path: 'enemies/new', component: EnemyCreationComponent },
      { path: 'enemies/:id/edit', component: EnemyCreationComponent },
      { path: 'items/new', component: ItemCreationComponent },
      { path: 'items/:id/edit', component: ItemCreationComponent }
    ]
  },

  { path: '**', redirectTo: '' } // Wildcard route for 404 page
];

import { Routes } from '@angular/router';
import { LayoutComponent } from './shared/ui/layout/layout.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', redirectTo: 'processos', pathMatch: 'full' },
      { 
        path: 'processos', 
        loadComponent: () => import('./features/processos/processo-list/processo-list.component').then(m => m.ProcessoListComponent) 
      },
      { 
        path: 'processos/novo', 
        loadComponent: () => import('./features/processos/processo-form/processo-form.component').then(m => m.ProcessoFormComponent) 
      },
      { 
        path: 'processos/:id', 
        loadComponent: () => import('./features/processos/processo-detail/processo-detail.component').then(m => m.ProcessoDetailComponent) 
      },
      { 
        path: 'entidades', 
        loadComponent: () => import('./features/entidades-legais/entidade-legal-list/entidade-legal-list.component').then(m => m.EntidadeLegalListComponent) 
      }
    ]
  },
  { path: '**', redirectTo: 'processos' }
];

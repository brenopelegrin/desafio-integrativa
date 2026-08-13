import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, NavigationEnd, Event } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  pageTitle = 'Processos';
  isSidebarCollapsed = false;

  constructor(private router: Router) {
    this.router.events.pipe(
      filter((event: Event): event is NavigationEnd => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      if (event.urlAfterRedirects.includes('/entidades')) {
        this.pageTitle = 'Entidades Legais';
      } else if (event.urlAfterRedirects.includes('/processos')) {
        this.pageTitle = 'Processos';
      } else {
        this.pageTitle = 'Processos';
      }
    });
  }

  toggleSidebar() {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}

import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'School';
  currentRoute: string = '';

  constructor(private router: Router) {
    this.router.events.subscribe(() => {
      this.currentRoute = this.router.url;
    });
  }
  isFullScreenRoute(): boolean {
    const fullScreenRoutes = ['confirm-email'];

    return fullScreenRoutes.some((route) => this.currentRoute.includes(route));
  }
}

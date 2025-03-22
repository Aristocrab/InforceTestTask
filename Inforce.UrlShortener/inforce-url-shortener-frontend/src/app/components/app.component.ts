import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { environment } from './../../environments/environment';
import { AuthService } from '../services/auth.service';
import { User } from '../models/User';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink],
  templateUrl: './app.component.html'
})
export class AppComponent {
  protected apiUrl: string = environment.apiUrl;
  protected currentUser: User | null = null;

  constructor(private authService: AuthService) {
    this.authService.getCurrentUser().subscribe((user) => {
      this.currentUser = user;
    });
  }

  protected logout() {
    this.authService.logout();
    window.location.href = this.apiUrl;
  }
}

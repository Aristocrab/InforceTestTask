import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { environment } from '../../environments/environment';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl: string = `${environment.apiUrl}/api/auth`;

  constructor(private cookieService: CookieService, private http: HttpClient) { }

  getCurrentUser() {
    return this.http.get<User>(`${this.apiUrl}`);
  }

  getToken() {
    return this.cookieService.get('jwt');
  }

  logout() {
    this.cookieService.delete('jwt');
  }
}

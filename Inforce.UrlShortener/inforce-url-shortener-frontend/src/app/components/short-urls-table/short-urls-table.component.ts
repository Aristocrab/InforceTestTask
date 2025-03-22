import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { ShortUrl } from '../../models/ShortUrl';
import { environment } from './../../../environments/environment';
import { User } from '../../models/User';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from "@angular/forms";
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-short-urls-table',
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './short-urls-table.component.html',
})
export class ShortUrlsTableComponent {

  protected shortUrls: ShortUrl[] = [];
  protected apiUrl: string = environment.apiUrl;
  protected currentUser: User | null = null;

  protected newUrl: string = '';

  constructor(private apiService: ApiService, private authService: AuthService) {
    this.apiService.getShortUrls().subscribe((shortUrls) => {
      this.shortUrls = shortUrls;
    });

    this.authService.getCurrentUser().subscribe((user) => {
      this.currentUser = user;
    });
  }

  async addUrl() {
    if (!this.newUrl) return;

    this.apiService.getShortCode(this.newUrl).subscribe((url) => {
      const shortCode = url.split("/").pop(); // Extract the last part

      if (shortCode) {
        this.apiService.getShortUrls().subscribe((shortUrls) => {
          this.shortUrls = shortUrls; // Update the list after successful addition
        });
      }
    });
  }

  deleteUrl(id: string) {
    if (this.currentUser?.role === 'Admin' || this.currentUser?.id === this.shortUrls.find(url => url.id === id)?.createdBy.id) {
      this.apiService.deleteUrl(id).subscribe(() => {
        this.shortUrls = this.shortUrls.filter(url => url.id !== id); // Remove deleted URL from the list
      });
    } else {
      alert("You do not have permission to delete this URL.");
    }
  }
}
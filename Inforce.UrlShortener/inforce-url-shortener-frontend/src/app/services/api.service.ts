import { Injectable } from '@angular/core';
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ShortUrl } from '../models/ShortUrl';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private apiUrl: string = `${environment.apiUrl}/api/urls`;

  constructor(private http: HttpClient) { }

  getShortUrls(): Observable<ShortUrl[]> {
    return this.http.get<ShortUrl[]>(this.apiUrl);
  }

  getShortCode(originalUrl: string): Observable<string> {
    const url = `${this.apiUrl}/short-code?originalUrl=${encodeURIComponent(originalUrl)}`;
    return this.http.post(url, null, { responseType: 'text' });
  }

  getOriginalUrl(shortUrl: string): Observable<string> {
    const url = `${this.apiUrl}/original-url?shortUrl=${encodeURIComponent(shortUrl)}`;
    return this.http.get(url, { responseType: 'text' });
  }

  getUrlInfoById(urlId: string): Observable<ShortUrl> {
    const url = `${this.apiUrl}/info?urlId=${urlId}`;
    return this.http.get<ShortUrl>(url);
  }

  getUrlInfoByCode(shortCode: string): Observable<ShortUrl> {
    const url = `${this.apiUrl}/info/${shortCode}`;
    return this.http.get<ShortUrl>(url);
  }

  deleteUrl(urlId: string): Observable<any> {
    const url = `${this.apiUrl}/${urlId}`;
    return this.http.delete(url);
  }
}

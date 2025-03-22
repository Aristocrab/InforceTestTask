import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { ShortUrl } from '../../models/ShortUrl';
import { environment } from './../../../environments/environment';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-short-url-info',
  imports: [DatePipe],
  templateUrl: './short-url-info.component.html'
})
export class ShortUrlInfoComponent {

  protected url: ShortUrl | null = null;
  protected apiUrl: string = environment.apiUrl;

  constructor(private route: ActivatedRoute, private apiService: ApiService) {
    this.route.params.subscribe(params => {
      const urlId = params['id'];
      this.apiService.getUrlInfoById(urlId).subscribe((url) => {
        this.url = url;
      });
    });
  }

}

import { Routes } from '@angular/router';
import { ShortUrlsTableComponent } from './components/short-urls-table/short-urls-table.component';
import { ShortUrlInfoComponent } from './components/short-url-info/short-url-info.component';

export const routes: Routes = [
    { path: '', component: ShortUrlsTableComponent, pathMatch: 'full' },
    { path: 'url/:id', component: ShortUrlInfoComponent },
];

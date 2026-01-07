//src/app/home.component.ts
import { Component, inject } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { environment } from './environments/environment';

@Component({
    selector: 'app-home',
    template:
    `<h1>Welcome to the Job Portal - Step 1</h1>
    <p> API Health: <strong [style.color]="healthy ? 'green' : 'red'">{{ healthy ? 'green' : 'red'"{{healthyText}} </strong></p>
    <p> Server Time: {{ serverTimeUtc || '...' }}</p>
    `
})
export class HomeComponent {
    private http = inject(HttpClient);
    healthy = false;
    healthyText = 'Checking...';
    serverTimeUtc?: string ;

    
ngOnInit() {
    // Health check
    this.http.get(`${environment.apiUrl}/health`, { responseType: 'text' })
      .subscribe({
        next: res => { this.healthy = true; this.healthyText = 'Healthy'; },
        error: _ => { this.healthy = false; this.healthyText = 'Unhealthy'; }
      });


// Ping endpoint using DI-backed time
    this.http.get<{ message: string, serverTimeUtc: string }>(`${environment.apiUrl}/api/ping`)
      .subscribe(res => this.serverTimeUtc = res.serverTimeUtc);


    }
}
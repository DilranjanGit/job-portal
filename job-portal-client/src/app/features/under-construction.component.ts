
import { Component } from '@angular/core';

@Component({
  selector: 'app-under-construction',
  standalone: true,
  template: `
    <div class="construction-box">
      <h1>🚧 Page is under construction 🚧</h1>
      <p>We are working hard to bring this page live soon.</p>
    </div>
  `,
  styles: [`
    .construction-box {
      text-align: center;
      margin-top: 60px;
      padding: 25px;
      max-width: 600px;
      margin-left: auto;
      margin-right: auto;
      background: #fff3cd;
      border: 1px solid #ffeeba;
      border-radius: 8px;
      color: #b85600;
      font-size: 22px;
      box-shadow: 0px 4px 10px rgba(0,0,0,0.1);
    }
    h1 {
      margin-bottom: 10px;
    }
  `]
})
export class UnderConstructionComponent {}

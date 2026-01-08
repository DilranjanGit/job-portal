//src/app/app.config.ts

import { ApplicationConfig } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import {routes} from './app.routes';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { HomeComponent } from './home.component';


export const appConfig: ApplicationConfig = {
  providers: [
        provideRouter(routes),
        provideHttpClient(withInterceptors([jwtInterceptor]))
  ]
};

import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

// services
import { ChangesService } from './changes/changes.service';
import { PriceService } from './price/price.service';
import { ReminderService } from './reminder/reminder.service';

import { AppComponent } from './app.component';
import { ChangesComponent } from './changes/changes.component';
import { PriceComponent } from './price/price.component';
import { ReminderComponent } from './reminder/reminder.component';
import { RequestOptions, HttpModule } from '@angular/http';
import { AuthRequestOptions } from './auth/auth.request-options';
import { AuthErrorHandler } from './auth/auth.error-handler';
import { HomeComponent } from './home/home.component';
import { Routes, RouterModule } from '@angular/router';
import { AuthService } from './auth/auth.service';
import { RegisterComponent } from './register/register.component';
import { AuthAlertService } from './auth/alert.service';
import { AlertComponent } from './auth/alert.component';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { LoginComponent } from './login/login.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth/auth.interceptor';
import { ConnectionService } from './connection/connection.service';
import { NoticesComponent } from './notices/notices.component';
import { NoticesService } from './notices/notices.service';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    RegisterComponent,
    LoginComponent,
    HomeComponent,
    ChangesComponent,
    PriceComponent,
    ReminderComponent,
    AlertComponent,
    NoticesComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpModule,
    NgbModule.forRoot(),
    RouterModule.forRoot(routes, {}),
  ],
  providers: [
    // {
    //   provide: RequestOptions,
    //   useClass: AuthRequestOptions
    // },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    {
      provide: ErrorHandler,
      useClass: AuthErrorHandler
    },
    ConnectionService,
    ChangesService,
    PriceService,
    ReminderService,
    AuthService,
    AuthAlertService,
    NoticesService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

// platformBrowserDynamic().bootstrapModule(AppModule);

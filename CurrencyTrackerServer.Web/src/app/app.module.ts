import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';
import { Ng2GoogleChartsModule } from 'ng2-google-charts';
import { InlineEditorModule } from '@qontu/ngx-inline-editor';
import { WebStorageModule } from 'ngx-store';

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
import { ChangesStatsComponent } from './stats/changesStats/changesStats.component';
import { StatsComponent } from './stats/stats.component';
import { ChangesStatsService } from './stats/changesStats/changesStats.service';
import { AdminService } from './admin/admin.service';
import { AdminComponent } from './admin/admin.component';
import { AccountComponent } from './account/account.component';


const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'logout', component: LoginComponent },
  { path: 'stats', component: StatsComponent },
  { path: 'admin', component: AdminComponent },
  { path: 'account', component: AccountComponent }
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
    NoticesComponent,
    StatsComponent,
    ChangesStatsComponent,
    AdminComponent,
    AccountComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpModule,
    NgbModule.forRoot(),
    Ng2GoogleChartsModule,
    WebStorageModule,
    InlineEditorModule,
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
    NoticesService,
    ChangesStatsService,
    AdminService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

// platformBrowserDynamic().bootstrapModule(AppModule);

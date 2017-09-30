import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { $WebSocket } from 'angular2-websocket/angular2-websocket';

// services
import { ChangesService } from './changes/changes.service';
import { PriceService } from './price/price.service';

import { AppComponent } from './app.component';
import { ChangesComponent } from './changes/changes.component';
import { PriceComponent } from './price/price.component';







@NgModule({
  declarations: [
    AppComponent,
    ChangesComponent,
    PriceComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    NgbModule.forRoot()
  ],
  providers: [ChangesService, PriceService],
  bootstrap: [AppComponent]
})
export class AppModule { }

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { ChangesComponent } from './changes/changes.component';

// services
import { ChangesService } from './changes/changes.service';
import {$WebSocket} from 'angular2-websocket/angular2-websocket';

import {NgbModule} from '@ng-bootstrap/ng-bootstrap';



@NgModule({
  declarations: [
    AppComponent,
    ChangesComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    NgbModule.forRoot()
  ],
  providers: [ChangesService],
  bootstrap: [AppComponent]
})
export class AppModule { }

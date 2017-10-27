import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

// services
import { ChangesService } from './changes/changes.service';
import { PriceService } from './price/price.service';
import { ReminderService } from './reminder/reminder.service';

import { AppComponent } from './app.component';
import { ChangesComponent } from './changes/changes.component';
import { PriceComponent } from './price/price.component';
import { ReminderComponent } from './reminder/reminder.component';

@NgModule({
  declarations: [
    AppComponent,
    ChangesComponent,
    PriceComponent,
    ReminderComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    NgbModule.forRoot()
  ],
  providers: [ChangesService, PriceService, ReminderService],
  bootstrap: [AppComponent]
})
export class AppModule { }

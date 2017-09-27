import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';

import { isDevMode } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { $WebSocket } from 'angular2-websocket/angular2-websocket';
import { HttpClient } from '@angular/common/http';



export enum ChangeType {
  Currency, Error, Info
}
export enum ChangeSource {
  None, Bittrex, Poloniex
}
export interface Change {
  currency?: string;
  time?: string;
  percentage?: number;
  threshold?: number;
  type?: ChangeType;
  changeSource?: ChangeSource;
  message?: string;
  recentlyChanged?: boolean;
}

export interface ChangeSettings {
  periodSeconds: number;
  percentage: number;
  resetHours: number;
  multipleChanges: boolean;
  multipleChangesSpanMinutes: number;
  marginPercentage: number;
  marginCurrencies: string[];
}

@Injectable()
export class ChangesService {

  public changes: Subject<Change[]> = new Subject<Change[]>();

  public subject: Subject<any>;
  private ws: $WebSocket;

  url: string = 'ws://' + window.location.host + '/changeNotifications';

  constructor(private http: HttpClient) {
    if (isDevMode()) {
      this.url = 'ws://localhost:5000/changeNotifications';
    }

    this.ws = new $WebSocket(this.url);


    this.subject = <Subject<Change[]>>this.ws.getDataStream().map((response: MessageEvent): Change[] => {
      const data = JSON.parse(response.data);
      return data;
    });
  }

  public getSettings(source: ChangeSource) {
    return this.http.get('/api/changes/settings/' + source).map(data => {
      const settings = data as ChangeSettings;
      if (!settings.marginCurrencies) {
        settings.marginCurrencies = [];
      }
      return settings;
    }).toPromise();
  }

  public saveSettings(source: ChangeSource, settings: ChangeSettings) {
    return this.http.post('/api/changes/settings/' + source, settings).map(data => data).toPromise();
  }

  public getHistory(source: ChangeSource): any {
    return this.http.get('/api/changes/' + source).map(data => data as Change[]).toPromise();
  }

  public reset(source: ChangeSource): any {
    return this.http.post('/api/changes/reset/' + source, null).toPromise();
  }

}



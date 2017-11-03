import { Injectable, Inject } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/retryWhen';
import 'rxjs/add/operator/delay';

import { isDevMode } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { HttpClient } from '@angular/common/http';
import { Source, ChangeType } from '../shared';

import { QueueingSubject } from 'queueing-subject';
import websocketConnect from 'rxjs-websockets';

export interface Change {
  currency?: string;
  time?: string;
  percentage?: number;
  threshold?: number;
  type?: ChangeType;
  changeSource?: Source;
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

  public input = new QueueingSubject<string>();
  public subject: Subject<any>;
  private messages: Observable<string>;

  url: string = 'ws://' + window.location.host + '/changeNotifications';

  constructor(private http: HttpClient) {
    if (isDevMode()) {
      this.url = 'ws://localhost:5000/changeNotifications';
    }

    this.connectSocket();
    this.mapSocketToSubject();

  }

  private mapSocketToSubject() {
    this.subject = <Subject<Change[]>>this.messages
      .retryWhen(errors => errors.delay(30000))
      .map((message: string): Change[] => {
        const data = JSON.parse(message);
        return data;
      });
  }

  private connectSocket() {
    try {
      this.messages = websocketConnect(this.url, this.input).messages.share();
    } catch (e) {
      console.log(e);
      return;
    }
  }

  public getSettings(source: Source) {
    return this.http.get('/api/changes/settings/' + source).map(data => {
      const settings = data as ChangeSettings;
      if (!settings.marginCurrencies) {
        settings.marginCurrencies = [];
      }
      return settings;
    }).toPromise();
  }

  public saveSettings(source: Source, settings: ChangeSettings) {
    return this.http.post('/api/changes/settings/' + source, settings, { responseType: 'text' }).map(data => data).toPromise();
  }

  public getHistory(source: Source): any {
    return this.http.get('/api/changes/' + source).map(data => data as Change[]).toPromise();
  }

  public reset(source: Source): any {
    return this.http.post('/api/changes/reset/' + source, null, { responseType: 'text' }).toPromise();
  }

}



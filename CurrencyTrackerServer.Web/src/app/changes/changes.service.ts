import { Injectable, Inject } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/retryWhen';
import 'rxjs/add/operator/delay';

import { isDevMode } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { HttpClient } from '@angular/common/http';
import { UpdateSource, UpdateType } from '../shared';

import { QueueingSubject } from 'queueing-subject';
import { ConnectionService, BaseChangeEntity } from '../connection/connection.service';

export interface Change {
  currency?: string;
  time?: string;
  percentage?: number;
  threshold?: number;
  type?: UpdateType;
  source?: UpdateSource;
  message?: string;
  recentlyChanged?: boolean;
}

export interface ChangeSettings {
  percentage: number;
  resetHours: number;
  multipleChanges: boolean;
  multipleChangesSpanMinutes: number;
  marginPercentage: number;
  marginCurrencies: string[];
}

@Injectable()
export class ChangesService {

  public subject: Subject<any>;
  private messages: Observable<string>;


  constructor(private http: HttpClient, private connection: ConnectionService) {

    this.mapSubject();

  }

  private mapSubject() {
    this.subject = <Subject<Change[]>>this.connection.changes
      .map((message: BaseChangeEntity[]): Change[] => {
        return message;
      });
  }


  public getSettings(source: UpdateSource) {
    return this.http.get('/api/changes/settings/' + source).map(data => {
      const settings = data as ChangeSettings;
      if (!settings.marginCurrencies) {
        settings.marginCurrencies = [];
      }
      return settings;
    }).toPromise();
  }

  public saveSettings(source: UpdateSource, settings: ChangeSettings) {
    return this.http.post('/api/changes/settings/' + source, settings, { responseType: 'text' }).map(data => data).toPromise();
  }

  public getHistory(source: UpdateSource): any {
    return this.http.get('/api/changes/' + source).map(data => data as Change[]).toPromise();
  }

  public reset(source: UpdateSource): any {
    return this.http.post('/api/changes/reset/' + source, null, { responseType: 'text' }).toPromise();
  }

}



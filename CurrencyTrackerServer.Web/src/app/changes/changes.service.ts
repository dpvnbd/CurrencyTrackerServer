import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/filter';
import { isDevMode } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { $WebSocket } from 'angular2-websocket/angular2-websocket';



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

@Injectable()
export class ChangesService {

  public changes: Subject<Change[]> = new Subject<Change[]>();

  public subject: Subject<any>;
  private ws: $WebSocket;

  url: string = 'ws://' + window.location.host + '/changeNotifications';

  constructor() {
    if (isDevMode()) {
      this.url = 'ws://localhost:5000/changeNotifications';
    }

    this.ws = new $WebSocket(this.url);


    this.subject = <Subject<Change[]>>this.ws.getDataStream().map((response: MessageEvent): Change[] => {
      const data = JSON.parse(response.data);
      return data;
    });
  }
}



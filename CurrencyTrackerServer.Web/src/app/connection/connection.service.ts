import { Injectable } from '@angular/core';

import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/retryWhen';
import 'rxjs/add/operator/delay';

import { isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UpdateSource, UpdateType, UpdateDestination } from '../shared';

import { Price } from '../price/price.service';
import { Change } from '../changes/changes.service';

import { QueueingSubject } from 'queueing-subject';
import websocketConnect from 'rxjs-websockets';

export interface BaseChangeEntity {
    time?: string;
    type?: UpdateType;
    source?: UpdateSource;
    destination: UpdateDestination;
    message?: string;
}


@Injectable()
export class ConnectionService {

    public input = new QueueingSubject<string>();

    public prices: Subject<BaseChangeEntity[]>;
    public changes: Subject<BaseChangeEntity[]>;
    public reminder: Subject<BaseChangeEntity[]>;
    public notices: Subject<BaseChangeEntity[]>;
    public connectionStatus: Subject<number>;
    private connectionStatusInternal: Observable<number>;

    public subject: Subject<any>;
    private messages: Observable<string>;

    url: string = 'ws://' + window.location.host + '/notifications';

    constructor(private http: HttpClient) {
        if (isDevMode()) {
            this.url = 'ws://localhost:5000/notifications';
        }
        this.prices = new Subject<BaseChangeEntity[]>();
        this.changes = new Subject<BaseChangeEntity[]>();
        this.reminder = new Subject<BaseChangeEntity[]>();
        this.notices = new Subject<BaseChangeEntity[]>();
        this.connectionStatus = new Subject<number>();
        this.reconnectSocket();
    }

    private mapSocketToSubject() {
        this.messages
            .retryWhen(errors => {
                return errors.delay(3000);
            })
            .subscribe((message: string) => {
                const data = JSON.parse(message);
                switch (data[0].destination) {
                    case UpdateDestination.CurrencyChange:
                        this.changes.next(data);
                        break;
                    case UpdateDestination.Price:
                        this.prices.next(data);
                        break;
                    case UpdateDestination.Reminder:
                        this.reminder.next(data);
                        break;
                    case UpdateDestination.Notice:
                        this.notices.next(data);
                        break;
                    default:
                        break;
                }
            });

        this.connectionStatusInternal.subscribe((n) => {
            this.connectionStatus.next(n);
        });
    }

    public reconnectSocket() {
        try {
            let token = '';
            this.getToken().then((data) => {
                token = data.token;
                const { messages, connectionStatus } = websocketConnect(this.url + '?token=' + token,
                    this.input);
                this.messages = messages.share();
                this.connectionStatusInternal = connectionStatus.share();
                this.messages = websocketConnect(this.url + '?token=' + token, this.input).messages.share();
                this.mapSocketToSubject();
            });

        } catch (e) {
            console.log(e);
            return;
        }
    }

    private getToken(): any {
        return this.http.post('/api/account/getToken', {}).toPromise();
    }
}



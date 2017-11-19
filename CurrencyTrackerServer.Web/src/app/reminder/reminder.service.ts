import { Injectable, isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/retryWhen';
import 'rxjs/add/operator/delay';
import { QueueingSubject } from 'queueing-subject';
import websocketConnect from 'rxjs-websockets';
import { Observable } from 'rxjs/Rx';
import { Subscription } from 'rxjs/Subscription';
import { ConnectionService, BaseChangeEntity } from '../connection/connection.service';

export interface ReminderSettings {
    period: number;
}

export interface ReminderNotification {
    time?: string;
}

@Injectable()
export class ReminderService {
    public subject: Subject<any>;
    public input = new QueueingSubject<string>();
    private messages: Observable<string>;


    constructor(private httpClient: HttpClient, private connection: ConnectionService) {
       this.mapSubject();

        const timer = Observable.timer(10000, 30 * 1000);
        timer.subscribe(t => {
            this.ping();
        });
    }

    private mapSubject() {
        this.subject = <Subject<ReminderNotification>>this.connection.reminder
            .map((message: BaseChangeEntity[]): ReminderNotification => {
                return message[0];
            });
    }


    public getSettings() {
        return this.httpClient.get('/api/reminder/period').map(data => {
            const settings = data as ReminderSettings;
            return settings;
        }).toPromise();
    }

    public saveSettings(settings: ReminderSettings) {
        return this.httpClient.post('/api/reminder/period', settings, { responseType: 'text' })
            .map(data => data).toPromise();
    }

    public start() {
        return this.httpClient.post('/api/reminder/start', { responseType: 'text' })
            .map(data => data).toPromise();
    }

    public ping() {
        return this.httpClient.post('/api/reminder/ping', { responseType: 'text' })
            .map(data => data).toPromise();
    }
}

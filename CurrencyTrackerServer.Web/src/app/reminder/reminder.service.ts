import { Injectable, isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/retryWhen';
import 'rxjs/add/operator/delay';
import { QueueingSubject } from 'queueing-subject';
import websocketConnect from 'rxjs-websockets';
import { Observable } from 'rxjs/Rx';
import { Subscription } from 'rxjs/Subscription';

export interface ReminderSettings {
    period: number;
}

export interface ReminderNotification {
    time: string;
}

@Injectable()
export class ReminderService {
    public subject: Subject<any>;
    public input = new QueueingSubject<string>();
    private messages: Observable<string>;

    url: string = 'ws://' + window.location.host + '/reminderNotifications';

    constructor(private httpClient: HttpClient) {
        if (isDevMode()) {
            this.url = 'ws://localhost:5000/reminderNotifications';
        }

        this.connectSocket();
        this.mapSocketToSubject();

        const timer = Observable.timer(10000, 60 * 1000);
        timer.subscribe(t => {
            this.ping();
        });
    }

    private mapSocketToSubject() {
        this.subject = <Subject<ReminderNotification>>this.messages
            .retryWhen(errors => errors.delay(30000))
            .map((message: string): ReminderNotification => {
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

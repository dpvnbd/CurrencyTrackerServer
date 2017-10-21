import { Injectable, isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs/Subject';
import { $WebSocket } from 'angular2-websocket/angular2-websocket';

export interface ReminderSettings {
    period: number;
}

export interface ReminderNotification {
    time: string;
}

@Injectable()
export class ReminderService {
    public subject: Subject<any>;
    private ws: $WebSocket;

    url: string = 'ws://' + window.location.host + '/reminderNotifications';

    constructor(private httpClient: HttpClient) {
        if (isDevMode()) {
            this.url = 'ws://localhost:5000/reminderNotifications';
        }

        this.ws = new $WebSocket(this.url);

        this.subject = <Subject<ReminderNotification>>this.ws.getDataStream()
            .map((response: MessageEvent): ReminderNotification => {
                const data = JSON.parse(response.data);
                return data;
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
}

import { Injectable, isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subject ,  Observable ,  Subscription, interval } from 'rxjs';
import { map } from 'rxjs/operators';
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

    constructor(private httpClient: HttpClient, private connection: ConnectionService) {
       this.mapSubject();

        const timer = interval(30 * 1000);
        timer.subscribe(t => {
            this.ping();
        });
    }

    private mapSubject() {
        this.subject = <Subject<ReminderNotification>>this.connection.reminder.pipe(
            map((message: BaseChangeEntity[]): ReminderNotification => {
                return message[0];
            }));
    }
    public getSettings() {
        return this.httpClient.get('/api/reminder/period').pipe(map(data => {
            const settings = data as ReminderSettings;
            return settings;
        })).toPromise();
    }

    public saveSettings(settings: ReminderSettings) {
        return this.httpClient.post('/api/reminder/period', settings, { responseType: 'text' })
            .toPromise();
    }

    public start() {
        return this.httpClient.post('/api/reminder/start', {}, { responseType: 'text' })
            .toPromise();
    }

    public ping() {
        return this.httpClient.post('/api/reminder/ping', {}, { responseType: 'text' })
            .toPromise();
    }
}

import { Injectable, isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UpdateSource, UpdateType, UpdateSpecial } from '../shared';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/retryWhen';
import 'rxjs/add/operator/delay';
import 'rxjs/add/operator/share';


import { QueueingSubject } from 'queueing-subject';
import websocketConnect from 'rxjs-websockets';
import { Observable } from 'rxjs/Observable';
import { BaseChangeEntity, ConnectionService } from '../connection/connection.service';

export interface Price {
    currency?: string;
    last?: number;
    high?: number;
    low?: number;

    source?: UpdateSource;
    type?: UpdateType;
    special?: UpdateSpecial;
    time?: string;
    message?: string;
    recentlyChanged?: boolean;
}

export interface PriceSettings {
    prices?: Price[];
    sendNotifications?: boolean;
    email?: string;
}

@Injectable()
export class PriceService {
    public prices: Subject<Price[]> = new Subject<Price[]>();
    public input = new QueueingSubject<string>();

    private messages: Observable<string>;
    public subject: Subject<any>;

    constructor(private http: HttpClient, private connection: ConnectionService) {
        this.mapSubject();
    }

    private mapSubject() {
        this.subject = <Subject<Price[]>>this.connection.prices
            .map((message: BaseChangeEntity[]): Price[] => {
                return message;
            });
    }

    public getSettings(source: UpdateSource) {
        return this.http.get('/api/price/settings/' + source).map(data => {
            const settings = data as PriceSettings;
            if (!settings.prices) {
                settings.prices = [];
            }
            return settings;
        }).toPromise();
    }


    public getPrice(source: UpdateSource, currency: string) {
        return this.http.get('/api/price/lastPrice/' + source + '/' + currency).map(data => {
            const price = data as Price;
            return price;
        }).toPromise();
    }

    public saveSettings(source: UpdateSource, settings: PriceSettings) {
        return this.http.post('/api/price/settings/' + source, settings, { responseType: 'text' }).map(data => data).toPromise();
    }

    public setNotification(source: UpdateSource, enabled: boolean) {
        return this.http.post('/api/price/notification/' + source + '/' + enabled, {}, { responseType: 'text' })
            .toPromise();
    }
}

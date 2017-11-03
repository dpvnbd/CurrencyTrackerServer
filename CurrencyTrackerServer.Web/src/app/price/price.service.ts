import { Injectable, isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Source, ChangeType } from '../shared';
import { Subject } from 'rxjs/Subject';
import 'rxjs/add/operator/retryWhen';
import 'rxjs/add/operator/delay';
import 'rxjs/add/operator/share';


import { QueueingSubject } from 'queueing-subject';
import websocketConnect from 'rxjs-websockets';
import { Observable } from 'rxjs/Observable';

export interface Price {
    currency?: string;
    last?: number;
    high?: number;
    low?: number;

    source?: Source;
    type?: ChangeType;
    time?: string;
    message?: string;
    recentlyChanged?: boolean;
}

export interface PriceSettings {
    periodSeconds: number;
    prices?: Price[];
}

@Injectable()
export class PriceService {
    public prices: Subject<Price[]> = new Subject<Price[]>();
    public input = new QueueingSubject<string>();

    private messages: Observable<string>;
    public subject: Subject<any>;

    url: string = 'ws://' + window.location.host + '/priceNotifications';
    constructor(private http: HttpClient) {
        if (isDevMode()) {
            this.url = 'ws://localhost:5000/priceNotifications';
        }

        this.connectSocket();
        this.mapSocketToSubject();
    }

    private mapSocketToSubject() {
        this.subject = <Subject<Price[]>>this.messages
            .retryWhen(errors => errors.delay(30000))
            .map((message: string): Price[] => {
                const data = JSON.parse(message);
                return data;
            });
    }

    private connectSocket() {
        try {
            this.messages = websocketConnect(this.url, this.input).messages;
        } catch (e) {
            console.log(e);
            return;
        }
    }

    public getSettings(source: Source) {
        return this.http.get('/api/price/settings/' + source).map(data => {
            const settings = data as PriceSettings;
            if (!settings.prices) {
                settings.prices = [];
            }
            return settings;
        }).toPromise();
    }


    public getPrice(source: Source, currency: string) {
        return this.http.get('/api/price/lastPrice/' + source + '/' + currency).map(data => {
            const price = data as Price;
            return price;
        }).toPromise();
    }

    public getPrices(source: Source) {
        return this.http.get('/api/price/' + source).map(data => {
            const prices = data as Price[];
            return prices;
        }).toPromise();
    }

    public saveSettings(source: Source, settings: PriceSettings) {
        return this.http.post('/api/price/settings/' + source, settings, { responseType: 'text' }).map(data => data).toPromise();
    }

    public start(source: Source) {
        return this.http.post('/api/price/settings/' + source, { responseType: 'text' })
            .map(data => data).toPromise();
    }

}

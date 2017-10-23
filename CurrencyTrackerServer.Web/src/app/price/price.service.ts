import { Injectable, isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Source, ChangeType } from '../shared';
import { Subject } from 'rxjs/Subject';
import { $WebSocket } from 'angular2-websocket/angular2-websocket';

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

    public subject: Subject<any>;
    private ws: $WebSocket;

    url: string = 'ws://' + window.location.host + '/priceNotifications';
    constructor(private http: HttpClient) {
        if (isDevMode()) {
            this.url = 'ws://localhost:5000/priceNotifications';
        }
        this.ws = new $WebSocket(this.url);


        this.subject = <Subject<Price[]>>this.ws.getDataStream().map((response: MessageEvent): Price[] => {
            const data = JSON.parse(response.data);
            return data;
        });
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

}

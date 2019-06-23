import { Injectable, isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UpdateSource, UpdateType, UpdateSpecial } from '../shared';
import { Subject ,  Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import websocketConnect from 'rxjs-websockets';
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

    public subject: Subject<any>;

    constructor(private http: HttpClient, private connection: ConnectionService) {
        this.mapSubject();
    }

    private mapSubject() {
        this.subject = <Subject<Price[]>>this.connection.prices.pipe(
            map((message: BaseChangeEntity[]): Price[] => {
                return message;
            }));
    }

    public getSettings(source: UpdateSource) {
        return this.http.get('/api/price/settings/' + source).pipe(map(data => {
            const settings = data as PriceSettings;
            if (!settings.prices) {
                settings.prices = [];
            }
            return settings;
        })).toPromise();
    }


    public getPrice(source: UpdateSource, currency: string) {
        return this.http.get('/api/price/lastPrice/' + source + '/' + currency).pipe(map(data => {
            const price = data as Price;
            return price;
        })).toPromise();
    }

    public saveSettings(source: UpdateSource, settings: PriceSettings) {
        return this.http.post('/api/price/settings/' + source, settings, { responseType: 'text' }).toPromise();
    }

    public saveCurrencies(source: UpdateSource, currencies: Price[]) {
        return this.http.post('/api/price/' + source, currencies, { responseType: 'text' }).toPromise();
    }

    public setNotification(source: UpdateSource, enabled: boolean) {
        return this.http.post('/api/price/notification/' + source + '/' + enabled, {}, { responseType: 'text' })
            .toPromise();
    }
}

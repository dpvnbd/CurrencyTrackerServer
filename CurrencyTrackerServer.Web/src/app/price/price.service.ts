import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Source } from '../shared';

export interface Price {
    currency?: string;
    source?: Source;
    message?: string;
    recentlyChanged?: boolean;
}

export interface PriceSettings {
    periodSeconds: number;
    prices: Price[];
}

@Injectable()
export class PriceService {
    constructor(private httpClient: HttpClient) { }

}
import { Pipe, PipeTransform } from '@angular/core';

export enum UpdateSource {
    None, Bittrex, Poloniex
}

export enum UpdateDestination {
    None, CurrencyChange, Price, Reminder, News
}

export enum UpdateType {
    Currency, Error, Info, Special
}

export enum UpdateSpecial {
    None, NotificationsEnabled, NotificationsDisabled
}

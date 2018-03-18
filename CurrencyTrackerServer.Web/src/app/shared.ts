import { Pipe, PipeTransform } from '@angular/core';

export enum UpdateSource {
    None, Bittrex, Poloniex
}

export enum UpdateDestination {
    None, CurrencyChange, Price, Reminder, Notice
}

export enum UpdateType {
    Currency, Error, Info, Special, Stats
}

export enum UpdateSpecial {
    None, NotificationsEnabled, NotificationsDisabled
}

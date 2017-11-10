import { Component, OnInit, ViewChild, Input, ElementRef } from '@angular/core';
import { UpdateSource, UpdateType, UpdateSpecial } from '../shared';
import { PriceService, Price, PriceSettings } from './price.service';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';


@Component({
    selector: 'app-price',
    templateUrl: 'price.component.html',
    styleUrls: ['price.component.css']
})

export class PriceComponent implements OnInit {
    @Input()
    source: UpdateSource;

    @ViewChild('bottom') bottom: ElementRef;

    prices: Price[] = [];
    settings: PriceSettings;
    sendNotification = false;
    lastError: Price;

    addedCurrency: any = {};

    private skipSpeech = true;
    linkTemplate: string;
    iconPath: string;
    soundEnabled = true;

    tempMute = false;

    audioHigh: HTMLAudioElement;
    audioLow: HTMLAudioElement;

    constructor(private priceService: PriceService, private modalService: NgbModal) { }

    ngOnInit() {
        this.audioHigh = new Audio();
        this.audioLow = new Audio();

        this.audioLow.src = '../../assets/sounds/low.wav';
        this.audioHigh.src = '../../assets/sounds/high.wav';

        this.audioLow.load();
        this.audioHigh.load();

        this.audioHigh.loop = true;
        this.audioLow.loop = true;

        if (this.source === UpdateSource.Bittrex) {
            this.linkTemplate = 'https://bittrex.com/Market/Index?MarketName=BTC-';
            this.iconPath = '../../assets/images/bittrexIcon.png';
        } else if (this.source = UpdateSource.Poloniex) {
            this.linkTemplate = 'https://poloniex.com/exchange#btc_';
            this.iconPath = '../../assets/images/poloniexIcon.png';
        }


        this.priceService.subject.subscribe((prices: Price[]) => {
            const localPrices: Price[] = [];
            for (const price of prices) {
                if (price.source === this.source) {
                    if (price.type === UpdateType.Error) {
                        price.time = Date.now().toString();
                        this.lastError = price;
                    } else if (price.type === UpdateType.Special) {
                        if (price.special === UpdateSpecial.NotificationsEnabled) {
                            this.sendNotification = true;
                        } else if (price.special === UpdateSpecial.NotificationsDisabled) {
                            this.sendNotification = false;
                        }
                    } else if (price.currency && price.last) {
                        localPrices.push(price);
                    }
                }
            }
            if (localPrices[0] && localPrices[0].source === this.source) {
                this.prices = localPrices;
                this.checkPriceBounds();
            }
        });

        this.priceService.getSettings(this.source).then((settings) => {
            if (settings && settings.prices) {
                this.prices = settings.prices;
                this.sendNotification = settings.sendNotifications;
            }
        });
    }

    checkPriceBounds() {
        let changed = false;
        let low = false;
        let high = false;
        for (const price of this.prices) {
            if (price.last <= price.low) {
                low = true;
                changed = true;
            } else if (price.last >= price.high) {
                high = true;
                changed = true;
            }
        }
        if (changed) {
            this.playAlarm(high, low);
        }
    }

    playAlarm(high: boolean, low: boolean) {
        if (!this.soundEnabled || this.tempMute) {
            return;
        }
        if (high) {
            this.audioHigh.play();
        } else if (low) {
            this.audioLow.play();
        }
    }

    openModal(content) {
        this.tempMute = true;
        this.audioHigh.pause();
        this.audioLow.pause();

        this.priceService.getSettings(this.source).then((settings) => {
            if (settings) {
                this.settings = settings;
                this.sendNotification = settings.sendNotifications;
                this.modalService.open(content).result.then((save) => {
                    this.tempMute = false;
                    if (save) {
                        this.priceService.saveSettings(this.source, this.settings);
                        this.prices = this.settings.prices;
                        this.sendNotification = this.settings.sendNotifications;
                    }
                });
            }
        });
    }

    getPrice() {
        if (!this.addedCurrency.currency) {
            return;
        }

        this.priceService.getPrice(this.source, this.addedCurrency.currency).then(
            (price) => {
                this.addedCurrency.high = this.addedCurrency.low = price.last.toFixed(8);
                this.addedCurrency.message = price.message;
            }
        );
    }

    addCurrencyToSettings() {
        if (this.addedCurrency.currency) {
            this.addedCurrency.currency = this.addedCurrency.currency.toUpperCase();

            const addedCurrencyClone = {
                currency: this.addedCurrency.currency,
                high: this.addedCurrency.high,
                low: this.addedCurrency.low,
            };

            for (const i in this.settings.prices) {
                if (this.settings.prices[i].currency === this.addedCurrency.currency) {
                    this.settings.prices.splice(parseInt(i, 10), 1, addedCurrencyClone);
                    return;
                }
            }
            this.settings.prices.push(addedCurrencyClone);
        }
    }

    removeCurrencyFromSettings(i: number) {
        this.settings.prices.splice(i, 1);
    }

    notificationChange(e) {
        this.priceService.setNotification(this.source, e.target.checked);
    }
}

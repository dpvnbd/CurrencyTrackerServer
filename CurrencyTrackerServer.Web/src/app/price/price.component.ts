import { Component, OnInit, ViewChild, Input, ElementRef } from '@angular/core';
import { Source } from '../shared';
import { PriceService, Price, PriceSettings } from './price.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';


@Component({
    selector: 'app-price',
    templateUrl: 'price.component.html'
})

export class PriceComponent implements OnInit {
    @Input()
    source: Source;

    @ViewChild('bottom') bottom: ElementRef;

    prices: Price[] = [];
    settings: PriceSettings;

    addedCurrency: Price = {};

    private skipSpeech = true;
    linkTemplate: string;
    iconPath: string;
    soundEnabled = true;

    constructor(private priceService: PriceService, private modalService: NgbModal) { }

    ngOnInit() {
        if (this.source === Source.Bittrex) {
            this.linkTemplate = 'https://bittrex.com/Market/Index?MarketName=BTC-';
            this.iconPath = '../../assets/images/bittrexIcon.png';
        } else if (this.source = Source.Poloniex) {
            this.linkTemplate = 'https://poloniex.com/exchange#btc_';
            this.iconPath = '../../assets/images/poloniexIcon.png';
        }

        this.priceService.subject.subscribe((prices: Price[]) => {
            const localPrices: Price[] = [];
            for (const price of prices) {
                if (price.source === this.source) {
                    localPrices.push(price);
                }
            }
            this.prices = localPrices;
        });

        this.priceService.getPrices(this.source).then((prices) => {
            if (prices) {
                this.prices = prices;
            }
        });
    }

    openModal(content) {
        this.priceService.getSettings(this.source).then((settings) => {
            if (settings) {
                this.settings = settings;

                this.modalService.open(content).result.then((save) => {
                    if (save) {
                        this.priceService.saveSettings(this.source, this.settings);
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
                this.addedCurrency.high = this.addedCurrency.low = price.last;
                this.addedCurrency.message = price.message;
            }
        );
    }

    addCurrencyToSettings() {
        if (this.addedCurrency.currency) {
            this.addedCurrency.currency = this.addedCurrency.currency.toUpperCase();
            this.settings.prices.push(this.addedCurrency);
        }
    }

    removeCurrencyFromSettings(i: number) {
        this.settings.prices.splice(i, 1);
    }

}

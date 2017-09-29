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

    prices: Price[];
    settings: PriceSettings;

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
    }
}

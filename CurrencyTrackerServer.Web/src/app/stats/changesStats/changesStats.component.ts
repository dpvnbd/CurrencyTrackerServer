import { Component, OnInit, Input } from '@angular/core';
import { UpdateSource } from '../../shared';
import { ChangesStatsService, ChangePercentage } from './changesStats.service';

@Component({
    selector: 'app-changes-stats',
    templateUrl: 'changesStats.component.html'
})

export class ChangesStatsComponent implements OnInit {
    @Input()
    source: UpdateSource;

    iconPath: string;
    linkTemplate: string;
    changesChartData: any;

    constructor(private changesStatsService: ChangesStatsService) {

    }

    ngOnInit() {
        if (this.source === UpdateSource.Bittrex) {
            this.linkTemplate = 'https://bittrex.com/Market/Index?MarketName=BTC-';
            this.iconPath = '../../assets/images/bittrexIcon.png';
        } else if (this.source === UpdateSource.Poloniex) {
            this.linkTemplate = 'https://poloniex.com/exchange#btc_';
            this.iconPath = '../../assets/images/poloniexIcon.png';
        }

        this.changesStatsService.getPercentages(this.source).then((changes) => {
            this.changesChartData = this.changesStatsService.convertPercentagesToChartData(changes);

            // Height is hardcoded because chart doesn't fill the parent element
            this.changesChartData.options = {
                legend: { position: 'none' },
                histogram: { lastBucketPercentile: 10 },
                chartArea: { width: '90%', height: '80%' },
                height: Math.max(document.documentElement.clientHeight, window.innerHeight || 0) * 0.45
                 - this.convertRemToPixels(3)
            };
        });
    }

    private convertRemToPixels(rem): number {
        return rem * parseFloat(getComputedStyle(document.documentElement).fontSize);
    }


}

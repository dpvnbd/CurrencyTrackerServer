import { Component, OnInit, AfterViewChecked, Input, ElementRef, ViewChild } from '@angular/core';
import { ChangesService, Change, ChangeSettings, ChangeType } from './changes.service';
import { DatePipe, DecimalPipe } from '@angular/common';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { Source } from '../shared';

@Component({
    selector: 'app-changes',
    templateUrl: 'changes.component.html',
    styleUrls: ['changes.component.css']
})
export class ChangesComponent implements OnInit {


    @Input()
    source: Source;

    @ViewChild('bottom') bottom: ElementRef;
    changes: Change[] = [];
    settings: ChangeSettings;
    message: string;
    private skipSpeech = true;
    modalCloseResult: string;
    newCurrency: string;

    linkTemplate: string;
    iconPath: string;
    soundEnabled = true;

    audioPing: HTMLAudioElement;


    constructor(private changesService: ChangesService, private modalService: NgbModal) { }

    ngOnInit() {
        this.audioPing = new Audio('../../assets/sounds/ping.mp3');

        if (this.source === Source.Bittrex) {
            this.linkTemplate = 'https://bittrex.com/Market/Index?MarketName=BTC-';
            this.iconPath = '../../assets/images/bittrexIcon.png';
        } else if (this.source = Source.Poloniex) {
            this.linkTemplate = 'https://poloniex.com/exchange#btc_';
            this.iconPath = '../../assets/images/poloniexIcon.png';
        }

        this.changesService.subject.subscribe((changes: Change[]) => {
            const localChanges: Change[] = [];
            for (const change of changes) {
                if (change.changeSource === this.source) {
                    if (change.type === ChangeType.Info && this.soundEnabled) {
                        this.audioPing.play();
                    } else {
                        localChanges.push(change);
                    }
                }
            }
            if (localChanges.length > 0) {
                this.addChanges(localChanges);
                this.speakChanges(localChanges);
            }
        });

        this.reloadHistory();
    }

    openModal(content) {
        this.changesService.getSettings(this.source).then((settings) => {
            if (settings) {
                this.settings = settings;

                this.modalService.open(content).result.then((save) => {
                    if (save) {
                        this.changesService.saveSettings(this.source, this.settings);
                    }
                });
            }
        });
    }


    reloadHistory() {
        this.message = 'Загрузка истории...';
        this.changes.length = 0;
        this.skipSpeech = true;
        this.changesService.getHistory(this.source).then(
            (history) => {
                if (history) {
                    this.addChanges(history);
                }
                this.message = null;
            }
        );
    }

    addChanges(changes: Change[]) {
        for (const oldChange of this.changes) {
            oldChange.recentlyChanged = false;
        }

        for (const change of changes) {
            if (change.changeSource === this.source) {
                change.recentlyChanged = true;
                this.changes.push(change);
            }
        }
        if (changes && changes.length > 0) {
            this.scrollToBottom();
        }

    }

    speakChanges(changes: Change[]) {
        if (this.skipSpeech || !this.soundEnabled) {
            this.skipSpeech = false;
            return;
        }

        let text = '';
        for (const change of changes) {
            if (change.currency && change.currency.length > 0) {
                text += ' ' + change.currency;
            }
        }

        if (text.length > 0) {
            responsiveVoice.speak(text, 'Russian Female');
        }
    }

    scrollToBottom() {
        if (this.bottom.nativeElement) {
            // Timeout to let view load changes before scrolling
            if (this.bottom.nativeElement !== undefined) {
                setTimeout(() => { this.bottom.nativeElement.scrollIntoView(true); }, 200);
            }
        }
    }

    reset() {
        this.changesService.reset(this.source).then((response) => {
            this.reloadHistory();
        });
    }

    addCurrencyToMargins() {
        if (!this.newCurrency) {
            return;
        }

        if (!this.settings.marginCurrencies) {
            this.settings.marginCurrencies = [];
        }
        if (!this.settings.marginCurrencies.includes(this.newCurrency)) {
            this.settings.marginCurrencies.push(this.newCurrency);
        }
        this.newCurrency = '';
    }

    removeMargin(i: number) {
        this.settings.marginCurrencies.splice(i, 1);
    }
}

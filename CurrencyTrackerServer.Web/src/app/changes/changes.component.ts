import { Component, OnInit, AfterViewChecked, Input, ElementRef, ViewChild } from '@angular/core';
import { ChangesService, Change, ChangeSource, ChangeSettings } from './changes.service';
import { DatePipe, DecimalPipe } from '@angular/common';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-changes',
    templateUrl: 'changes.component.html',
    styleUrls: ['changes.component.css']
})
export class ChangesComponent implements OnInit {


    @Input()
    source: ChangeSource;

    @ViewChild('bottom') bottom: ElementRef;
    private changes: Change[] = [];
    private settings: ChangeSettings;
    private message: string;
    private skipSpeech = true;
    modalCloseResult: string;

    constructor(private changesService: ChangesService, private modalService: NgbModal) { }

    ngOnInit() {
        this.changesService.subject.subscribe((changes: Change[]) => {
            const localChanges: Change[] = [];
            for (const change of changes) {
                if (change.changeSource === this.source) {
                    localChanges.push(change);
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
        if (this.skipSpeech) {
            this.skipSpeech = false;
            return;
        }

        let text = '';
        for (const change of changes) {
            if (change.currency && change.currency.length > 0) {
                text += ' ' + change.currency;
            }
        }
        responsiveVoice.speak(text, 'Russian Female');
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
}

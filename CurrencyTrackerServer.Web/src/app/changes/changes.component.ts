import { Component, OnInit, AfterViewChecked, Input, ElementRef, ViewChild } from '@angular/core';
import { ChangesService, Change, ChangeSource } from './changes.service';
import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';


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

    private message: string;

    private skipSpeech = true;


    constructor(private changesService: ChangesService, private http: HttpClient) { }

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

    reloadHistory() {
        this.message = 'Загрузка истории...';
        this.changes.length = 0;

        this.http.get('/api/changes').subscribe(data => {
            const loaded = data as Change[];
            this.skipSpeech = true;
            this.addChanges(loaded);
            this.message = null;
        });

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
        if (changes.length > 0) {
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
            if (change.currency.length > 0) {
                text += ' ' + change.currency;
            }
        }
        responsiveVoice.speak(text, "Russian Female");
    }

    scrollToBottom() {
        if (this.bottom.nativeElement) {
            // I can't remember why I added a short timeout,
            // the below works great though.
            if (this.bottom.nativeElement !== undefined) {
                setTimeout(() => { this.bottom.nativeElement.scrollIntoView(true); }, 200);
            }
        }
    }

    reset() {
        this.http.post('/api/changes/reset/' + this.source, null).subscribe((response) => {
            this.reloadHistory();
        });
    }
}

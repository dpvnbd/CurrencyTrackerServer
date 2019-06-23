import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { UpdateSource, UpdateType } from '../shared';
import { Notice, NoticesService } from './notices.service';
import { Howl } from 'howler';
import { LocalStorage } from 'ngx-store';

@Component({
    selector: 'app-notices',
    templateUrl: 'notices.component.html'
})
export class NoticesComponent implements OnInit {

    @ViewChild('bottom') bottom: ElementRef;
    notices: Notice[] = [];
    lastError: Notice;
    @LocalStorage('noticesSound') soundEnabled = true;

    audio: Howl;
    audioPlaying = false;

    constructor(private noticesService: NoticesService) { }

    ngOnInit() {
        this.noticesService.getNotices().then((n: Notice[]) => {
            this.addChanges(n, false, true);
        });

        this.noticesService.subject.subscribe((changes: Notice[]) => {
            const localChanges: Notice[] = [];
            for (const change of changes) {
                if (change.type === UpdateType.Currency) {
                    localChanges.push(change);
                } else if (change.type === UpdateType.Error) {
                    this.lastError = change;
                    console.log(change.message);
                }
            }
            if (localChanges.length > 0) {
                this.addChanges(localChanges);
                this.playAlarm();
            }
        });

        this.initAudio();
    }

    addChanges(changes: Notice[], markAsNew = true, addToEnd = false) {
        for (const oldChange of this.notices) {
            oldChange.recentlyChanged = false;
        }

        for (const change of changes) {
            if (markAsNew) {
                change.recentlyChanged = true;
            }
            if (addToEnd) {
                this.notices.push(change);
            } else {
                this.notices.unshift(change);
            }
        }
        // if (changes && changes.length > 0) {
        //     this.scrollToBottom();
        // }

    }


    playAlarm() {
        if (!this.soundEnabled || this.audio.playing()) {
            return;
        }
        this.audio.play();
        this.audioPlaying = true;
    }

    stopAlarm() {
        this.audio.stop();
        this.audioPlaying = false;
        for (const notice of this.notices) {
            notice.recentlyChanged = false;
        }
    }


    initAudio() {
        this.audio = new Howl({
            src: ['../../assets/sounds/notice.mp3'],
            loop: true
        });
    }

    scrollToBottom() {
        if (this.bottom.nativeElement) {
            // Timeout to let view load changes before scrolling
            if (this.bottom.nativeElement !== undefined) {
                setTimeout(() => { this.bottom.nativeElement.scrollIntoView(true); }, 200);
            }
        }
    }
}

import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { UpdateSource, UpdateType } from '../shared';
import { Notice, NoticesService } from './notices.service';

@Component({
    selector: 'app-notices',
    templateUrl: 'notices.component.html'
})
export class NoticesComponent implements OnInit {

    @ViewChild('bottom') bottom: ElementRef;
    notices: Notice[] = [];
    lastError: Notice;
    soundEnabled = true;

    audio: HTMLAudioElement;

    constructor(private noticesService: NoticesService) { }

    ngOnInit() {
        this.noticesService.getNotices().then((n: Notice[]) => {
            this.addChanges(n, false);
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

    addChanges(changes: Notice[], markAsNew = true) {
        for (const oldChange of this.notices) {
            oldChange.recentlyChanged = false;
        }

        for (const change of changes) {
            if (markAsNew) {
                change.recentlyChanged = true;
            }
            this.notices.push(change);
        }
        if (changes && changes.length > 0) {
            this.scrollToBottom();
        }

    }


    playAlarm() {
        if (!this.soundEnabled) {
            return;
        }
        this.audio.play();
    }

    stopAlarm() {
        this.audio.pause();

        for (const notice of this.notices) {
            notice.recentlyChanged = false;
        }
    }


    initAudio() {

        this.audio = new Audio();
        this.audio.src = '../../assets/sounds/notice.mp3';
        this.audio.load();
        this.audio.loop = true;
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

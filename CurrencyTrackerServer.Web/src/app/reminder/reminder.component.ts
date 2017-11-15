import { Component, OnInit, ViewChild, Input, ElementRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ReminderService, ReminderSettings, ReminderNotification } from './reminder.service';

@Component({
    selector: 'app-reminder',
    templateUrl: 'reminder.component.html'
})

export class ReminderComponent implements OnInit {

    settings: ReminderSettings;
    soundEnabled = true;
    lastUpdate: string;
    constructor(private reminderService: ReminderService, private modalService: NgbModal,

    ngOnInit() {
        this.reminderService.subject.subscribe((notification: ReminderNotification) => {
            this.lastUpdate = notification.time;
            if (this.soundEnabled) {
                responsiveVoice.speak('working', 'Russian Female', { rate: 0.8 });
            }
        });

        this.reminderService.start();
    }

    openModal(content) {
        this.reminderService.getSettings().then((settings) => {
            if (settings) {
                this.settings = settings;
                this.modalService.open(content).result.then((save) => {
                    if (save) {
                        this.reminderService.saveSettings(this.settings);
                    }
                });
            }
        });
    }
}

import { Component, OnInit } from '@angular/core';

import { AuthAlertService } from './alert.service';

@Component({
    moduleId: module.id,
    selector: 'app-alert',
    templateUrl: 'alert.component.html'
})

export class AlertComponent implements OnInit {
    message: any;

    constructor(private alertService: AuthAlertService) { }

    ngOnInit() {
        this.alertService.getMessage().subscribe(message => { this.message = message; });
    }
}

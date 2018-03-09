import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { AuthAlertService } from '../auth/alert.service';
import { LocalStorage } from 'ngx-store/dist';

@Component({
    selector: 'app-account',
    templateUrl: 'account.component.html'
})

export class AccountComponent implements OnInit {
    @LocalStorage()
    userInfo: any = {
        username: 'unavailable',
        isAdmin: false
    };

    oldPassword: string;
    newPassword: string;
    confirmNewPassword: string;

    constructor(private authService: AuthService, private alertService: AuthAlertService) { }

    ngOnInit() {

    }

    changePassword() {
        if (this.newPassword !== this.confirmNewPassword) {
            this.alertService.error('Пароли не совпадают');
            return;
        }

        this.authService.changePassword(this.oldPassword, this.newPassword).then(success => {
            this.alertService.success('Пароль успешно изменен');
        },
            error => {
                this.alertService.error('Не удалось изменить пароль');
            });
    }
}

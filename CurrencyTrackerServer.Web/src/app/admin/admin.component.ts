import { Component, OnInit } from '@angular/core';
import { AdminService, UserInfo } from './admin.service';

@Component({
    selector: 'app-admin',
    templateUrl: 'admin.component.html'
})

export class AdminComponent implements OnInit {
    users: UserInfo[] = [];
    constructor(private adminService: AdminService) { }

    ngOnInit() {
        this.adminService.getUsers().then(users => {
            this.users = users;
        });
    }

    deleteUser(user: UserInfo) {
        if (window.confirm('Удалить пользователя ' + user.email + '?')) {
            this.adminService.deleteUser(user.id).then(success => {
                window.location.reload(true);
            });
        }
    }

    toggleUser(user: UserInfo) {
        this.adminService.setUserEnabled(user.id, !user.isEnabled).then(success => {
            window.location.reload(true);
        });
    }
}

import { Router, ActivatedRoute, RouterLink } from '@angular/router';

import { AuthAlertService, } from '../auth/alert.service';
import { AuthService } from '../auth/auth.service';

import {
    Component,
    OnInit
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { BrowserModule } from '@angular/platform-browser';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { LocalStorage } from 'ngx-store';


@Component({
    moduleId: module.id,
    templateUrl: 'login.component.html'
})

export class LoginComponent implements OnInit {
    returnUrl: string;

    myform: FormGroup;
    name: FormControl;
    password: FormControl;

    @LocalStorage()
    userInfo: any = {
        username: 'unavailable',
        isAdmin: false
    };

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthService,
        private alertService: AuthAlertService) { }

    ngOnInit() {
        this.createFormControls();
        this.createForm();
        // reset login status
        this.authenticationService.logout();
        this.userInfo = {};
        this.userInfo.save();
    }

    login() {
        if (!this.myform.valid) {
            return;
        }
        this.authenticationService.login(this.myform.value)
            .subscribe(
            data => {
                if (data['token']) {
                    this.authenticationService.setToken(data['token']);
                }
                this.userInfo = data;
                this.userInfo.save();
                // this.router.navigate(['']);

                // reload main page so app initializes with logged in user
                location.replace('');
            },
            error => {
                this.alertService.error('Неправильное имя пользователя или пароль');
            });
    }

    createFormControls() {
        this.name = new FormControl('', [
            Validators.required,
        ]);


        this.password = new FormControl('', [
            Validators.required,
        ]);
    }

    createForm() {
        this.myform = new FormGroup({
            name: this.name,
            password: this.password,
        });
    }
}

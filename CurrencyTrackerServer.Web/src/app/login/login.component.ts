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


@Component({
    moduleId: module.id,
    templateUrl: 'login.component.html'
})

export class LoginComponent implements OnInit {
    returnUrl: string;

    myform: FormGroup;
    name: FormControl;
    password: FormControl;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthService,
        private alertService: AuthAlertService) { }

    ngOnInit() {
        this.createFormControls();
        this.createForm();
        // reset login status
        // this.authenticationService.logout();
        // get return url from route parameters or default to '/'
        // this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    login() {
        if (!this.myform.valid) {
            return;
        }
        this.authenticationService.login(this.myform.value)
            .subscribe(
            data => {
                const json = data.json();

                if (json.token) {
                    this.authenticationService.setToken(json.token);
                }

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

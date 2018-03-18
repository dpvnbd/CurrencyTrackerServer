import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { AuthAlertService } from '../auth/alert.service';
import {
    Component,
    OnInit
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { BrowserModule } from '@angular/platform-browser';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

@Component({
    moduleId: module.id,
    templateUrl: 'register.component.html'
})

export class RegisterComponent implements OnInit {

    myform: FormGroup;
    name: FormControl;
    email: FormControl;
    password: FormControl;
    confirmPassword: FormControl;

    ngOnInit(): void {
        this.createFormControls();
        this.createForm();
    }
    constructor(
        private router: Router,
        private authService: AuthService,
        private alertService: AuthAlertService) { }

    register() {
        if (!this.myform.valid) {
            return;
        }
        this.authService.register(this.myform.value)
            .subscribe(
            data => {
                if (data.ok) {
                    this.alertService.success('Registration successful', true);
                    this.router.navigate(['/login']);
                }
            },
            error => {
                let message = '';
                if (error._body) {
                    message += error._body;
                } else {
                    message = error;
                }
                this.alertService.error(message);
            });
    }

    createFormControls() {
        this.name = new FormControl('', [
            Validators.required,
            Validators.minLength(4)
        ]);
        this.email = new FormControl('', [
            Validators.required,
            Validators.email
        ]);

        this.password = new FormControl('', [
            Validators.required,
            Validators.minLength(4)
        ]);

        this.confirmPassword = new FormControl('', [
            Validators.required,
        ]);
    }

    createForm() {
        this.myform = new FormGroup({
            name: this.name,
            email: this.email,
            password: this.password,
            confirmPassword: this.confirmPassword,
        });
    }
}

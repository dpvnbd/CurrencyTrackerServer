import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { LocalStorage } from 'ngx-store';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    @LocalStorage()
    userInfo: any = {
        username: 'unavailable',
        isAdmin: false
    };

    constructor() { }
    ngOnInit() {

    }
}

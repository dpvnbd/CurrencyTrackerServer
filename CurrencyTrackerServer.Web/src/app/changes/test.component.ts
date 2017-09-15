import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-test',
    template: `<li>
    <ul *ngFor="let item of items">{{item}}</ul>    
    </li>
    <input [(ngModel)]="message"> <button (click) = "addItem(message)">Add</button>`
})

export class TestComponent implements OnInit {
    items: string[] = [];
    message: string;
    constructor() { }

    addItem(item: string) {
        this.items.push(item);
    }
    ngOnInit() { }
}

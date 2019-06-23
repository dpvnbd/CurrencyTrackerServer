import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { isDevMode } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UpdateSource, UpdateType, UpdateDestination } from '../shared';
import { QueueingSubject } from 'queueing-subject';

import makeWebSocketObservable, {
    GetWebSocketResponses,
    normalClosureMessage,
} from 'rxjs-websockets';
import { retryWhen, delay, map, share, switchMap } from 'rxjs/operators';

export interface BaseChangeEntity {
    time?: string;
    type?: UpdateType;
    source?: UpdateSource;
    destination: UpdateDestination;
    message?: string;
}


@Injectable()
export class ConnectionService {
    public prices: Subject<BaseChangeEntity[]>;
    public changes: Subject<BaseChangeEntity[]>;
    public reminder: Subject<BaseChangeEntity[]>;
    public stats: Subject<BaseChangeEntity[]>;
    public notices: Subject<BaseChangeEntity[]>;
    public connectionStatus: Subject<number>;
    private connectionStatusInternal: Observable<number>;

    url: string = 'ws://' + window.location.host + '/notifications';

    input$ = null;
    socket$ = null;
    messages$: Observable<string> = null;

    constructor(private http: HttpClient) {
        if (isDevMode()) {
            this.url = 'ws://localhost:5000/notifications';
        }
        this.prices = new Subject<BaseChangeEntity[]>();
        this.changes = new Subject<BaseChangeEntity[]>();
        this.reminder = new Subject<BaseChangeEntity[]>();
        this.stats = new Subject<BaseChangeEntity[]>();
        this.notices = new Subject<BaseChangeEntity[]>();
        this.input$ = new QueueingSubject<string>();

        this.reconnectSocket();
    }

    private mapSocketToSubject() {
        this.messages$ = this.socket$.pipe(
                switchMap((getResponses: GetWebSocketResponses<string>) => getResponses(this.input$)),
                retryWhen(errors => errors.pipe(delay(3000))),
            );

        this.messages$.subscribe((message: string) => {
            const data = JSON.parse(message);
            switch (data[0].destination) {
                case UpdateDestination.CurrencyChange:
                    this.changes.next(data);
                    break;
                case UpdateDestination.Price:
                    this.prices.next(data);
                    break;
                case UpdateDestination.Reminder:
                    this.reminder.next(data);
                    break;
                case UpdateDestination.Notice:
                    this.notices.next(data);
                    break;
                default:
                    break;
            }
            if (data[0].type === UpdateType.Stats) {
                this.stats.next(data);
            }
        });
    }

    public reconnectSocket() {
        try {
            let token = '';
            this.getToken().then((data) => {
                token = data.token;
                this.socket$ = makeWebSocketObservable(this.url + '?token=' + token);
                this.mapSocketToSubject();
            });

        } catch (e) {
            console.log(e);
            return;
        }
    }

    private getToken(): any {
        return this.http.post('/api/account/getToken', {}).toPromise();
    }
}



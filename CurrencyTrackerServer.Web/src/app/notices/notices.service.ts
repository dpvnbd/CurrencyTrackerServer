import { Injectable, Inject } from '@angular/core';
import { Subject ,  Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { UpdateSource, UpdateType } from '../shared';

import { ConnectionService, BaseChangeEntity } from '../connection/connection.service';

export interface Notice {
  time?: string;
  type?: UpdateType;
  source?: UpdateSource;
  message?: string;
  recentlyChanged?: boolean;
}

@Injectable()
export class NoticesService {

  public subject: Subject<Notice[]>;
  private messages: Observable<string>;


  constructor(private http: HttpClient, private connection: ConnectionService) {
    this.mapSubject();
  }

  private mapSubject() {
    this.subject = <Subject<Notice[]>>this.connection.notices.pipe(
      map((message: BaseChangeEntity[]): Notice[] => {
        return message;
      }));
  }

  public getNotices(): any {
    return this.http.get('/api/notices/').pipe(map(data => data as Notice[])).toPromise();
  }
}



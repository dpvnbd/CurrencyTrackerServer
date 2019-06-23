import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { map} from 'rxjs/operators';

export interface UserInfo {
    id: string;
    email: string;
    username: string;
    isEnabled: boolean;
    isAdmin: boolean;
}

@Injectable()
export class AdminService {
    constructor(private httpClient: HttpClient) { }

    public getUsers(): any {
        return this.httpClient.get('/api/admin/users/')
            .pipe(map(data => data as UserInfo[])).toPromise();
    }

    public deleteUser(id: string): any {
        return this.httpClient.delete('/api/admin/users/' + id)
            .toPromise();
    }

    public setUserEnabled(id: string, enabled: boolean): any {
        return this.httpClient.put('/api/admin/users/' + id, { isEnabled: enabled })
            .toPromise();
    }
}

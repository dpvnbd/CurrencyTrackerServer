import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private auth: AuthService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        if (req.url.includes('Notifications')) {
            console.log('caught websocket');
        }
        // Get the auth header from the service.
        const authHeader = this.auth.getToken();
        // Clone the request to add the new header.
        const authReq = req.clone({ headers: req.headers.set('Authorization', 'Bearer ' + authHeader) });
        // Pass on the cloned request instead of the original request.
        return next.handle(authReq);
    }
}

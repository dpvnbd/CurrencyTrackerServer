import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';
import * as jwt_decode from 'jwt-decode';

export const TOKEN_NAME = 'jwt_token';

@Injectable()
export class AuthService {

  private url = 'api/account';
  private headers = new Headers({ 'Content-Type': 'application/json' });

  constructor(private http: Http) { }

  getToken(): string {
    return localStorage.getItem(TOKEN_NAME);
  }

  setToken(token: string): void {
    localStorage.setItem(TOKEN_NAME, token);
  }

  getTokenExpirationDate(token: string): Date {
    const decoded = jwt_decode(token);

    if (decoded.exp === undefined) {
      return null;
    }

    const date = new Date(0);
    date.setUTCSeconds(decoded.exp);
    return date;
  }

  isTokenExpired(token?: string): boolean {
    if (!token) { token = this.getToken(); }

    if (!token) { return true; }

    const date = this.getTokenExpirationDate(token);
    if (date === undefined) { return false; }
    return !(date.valueOf() > new Date().valueOf());
  }

  register(user) {
    return this.http
      .post(`${this.url}/register`, JSON.stringify(user), { headers: this.headers });
  }

  login(user) {
    return this.http
      .post(`${this.url}/login`, JSON.stringify(user), { headers: this.headers });
  }

  logout() {
    localStorage.removeItem(TOKEN_NAME);
  }
}

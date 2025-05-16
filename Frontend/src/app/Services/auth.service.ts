import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { IConfirmEmail } from '../Models/ConfirmRequestDTO.model';
import { tap } from 'rxjs/operators';
import { RegisterRequestDTO } from '../Models/RegisterRequestDTO .model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isLoggedInSubject = new BehaviorSubject<boolean>(false);
  isLoggedIn$: Observable<boolean> = this.isLoggedInSubject.asObservable();
  constructor(private http: HttpClient) {
    const token = this.getToken();
    this.isLoggedInSubject.next(!!token);
  }
  private readonly apiUrl: string = 'https://localhost:7073/api';

  private readonly TOKEN_KEY = 'auth_token';

  Register(request: RegisterRequestDTO): Observable<Response> {
    return this.http.post<Response>(
      `${this.apiUrl}/Authentication/Register`,
      request
    );
  }

  resendConfirmationEmail(email: string): Observable<Response> {
    return this.http.post<Response>(
      `${this.apiUrl}/Authentication/ResendConfirmEmail`,
      { email }
    );
  }

  ConfirmationEmail(confirmedDto: IConfirmEmail): Observable<Response> {
    return this.http.post<Response>(
      `${this.apiUrl}/Authentication/confirm-email`,
      confirmedDto
    );
  }

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/Authentication/Login`, credentials)
      .pipe(
        tap((response: any) => {
          if (response.data.taken) {
            this.setToken(response.data.taken);
            this.isLoggedInSubject.next(true);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.isLoggedInSubject.next(false);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }
}

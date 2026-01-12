import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { BaseResponse, UserDto } from '../models/api.models';

export interface RegisterUserRequest {
  userName: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'taskit_user';
  private readonly userSubject = new BehaviorSubject<UserDto | null>(this.readStoredUser());

  readonly user$ = this.userSubject.asObservable();

  constructor(private readonly http: HttpClient) {}

  get currentUser(): UserDto | null {
    return this.userSubject.value;
  }

  isAuthenticated(): boolean {
    return !!this.userSubject.value;
  }

  register(payload: RegisterUserRequest): Observable<BaseResponse<UserDto>> {
    return this.http
      .post<BaseResponse<UserDto>>(`${environment.apiBaseUrl}/Auth/register`, payload)
      .pipe(tap((res) => this.persistUser(res)));
  }

  login(payload: LoginRequest): Observable<BaseResponse<UserDto>> {
    return this.http
      .post<BaseResponse<UserDto>>(`${environment.apiBaseUrl}/Auth/login`, payload)
      .pipe(tap((res) => this.persistUser(res)));
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.userSubject.next(null);
  }

  private persistUser(response: BaseResponse<UserDto>): void {
    if (response.success && response.data) {
      localStorage.setItem(this.storageKey, JSON.stringify(response.data));
      this.userSubject.next(response.data);
    }
  }

  private readStoredUser(): UserDto | null {
    const raw = localStorage.getItem(this.storageKey);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as UserDto;
    } catch {
      localStorage.removeItem(this.storageKey);
      return null;
    }
  }
}

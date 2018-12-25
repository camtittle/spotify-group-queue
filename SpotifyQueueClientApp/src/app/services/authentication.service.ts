import { Injectable } from '@angular/core';
import { map, catchError } from 'rxjs/operators';
import { ApiService } from './api.service';
import { RegisterResponse } from '../models';
import { HttpErrorResponse } from '@angular/common/http';
import { throwError, Observable } from 'rxjs';
import {environment} from '../../environments/environment';
import { Register } from 'ts-node';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  public currentUser: RegisterResponse;

  constructor(private apiService: ApiService) { }

  public register(username: string): Observable<RegisterResponse> {
    if (!environment.production && environment.useDevRegisterEndpoint && environment.devPassword) {
      console.warn('Using dev token endpoint --> can register as anyone! Disable before deploying.');
      return this.apiService.post<RegisterResponse>('/auth/token', {username: username, developerPassword: environment.devPassword})
        .pipe(map(response => {
            this.currentUser = response;
            console.log(this.currentUser);
            this.saveAccessToken(response);
            return response;
          })
        );
    }
    return this.apiService.post<RegisterResponse>('/auth/register', {username: username})
      .pipe(map(response => {
          this.saveAccessToken(response);
          this.currentUser = response;
          return response;
        })
      );
  }

  private saveAccessToken(token: RegisterResponse) {
    sessionStorage.setItem('currentUser', JSON.stringify(token));
  }

  public getAccessToken(): RegisterResponse {
    return <RegisterResponse>JSON.parse(sessionStorage.getItem('currentUser'));
  }
}

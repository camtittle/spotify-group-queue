import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { AccessToken } from '../models';
import { Observable, BehaviorSubject } from 'rxjs';
import {environment} from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  public currentUser$ = new BehaviorSubject<AccessToken>(null);

  constructor(private apiService: ApiService) {
    // Initialise current user observable using stored credentials
    const user = this.getAccessToken();
    console.log(user);
    if (user) {
      this.currentUser$.next(user);
    }
  }

  public register(username: string): Observable<AccessToken> {
    console.log(environment)
    if (!environment.production && environment.useDevRegisterEndpoint && environment.devPassword) {
      console.warn('Using dev token endpoint --> can register as anyone! Disable before deploying.');
      return this.apiService.post<AccessToken>('/auth/token', {username: username, developerPassword: environment.devPassword})
        .pipe(map(response => {
            this.currentUser$.next(response);
            this.saveAccessToken(response);
            return response;
          })
        );
    }
    return this.apiService.post<AccessToken>('/auth/register', {username: username})
      .pipe(map(response => {
          this.saveAccessToken(response);
          this.currentUser$.next(response);
          return response;
        })
      );
  }

  public isAdmin(): boolean {
    const user = this.currentUser$.getValue();

    return user ? user.currentParty.owner.id === user.id : false;
  }

  private saveAccessToken(token: AccessToken) {
    sessionStorage.setItem('currentUser', JSON.stringify(token));
  }

  private getAccessToken(): AccessToken {
    return <AccessToken>JSON.parse(sessionStorage.getItem('currentUser'));
  }
}

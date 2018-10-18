import { Injectable } from '@angular/core';
import { map, catchError } from 'rxjs/operators';
import { ApiService } from './api.service';
import { RegisterResponse } from '../models';
import { HttpErrorResponse } from '@angular/common/http';
import { throwError, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private apiService: ApiService) { }

  register(username: string): Observable<RegisterResponse> {
    return this.apiService.post<RegisterResponse>('/auth/register', {username: username})
      .pipe(map(response => {
          localStorage.setItem('currentUser', JSON.stringify(response));
          return response;
        })
      );
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private http: HttpClient) { }

  register(username: string) {
    return this.http.post<any>('${BASE_API_URL}/auth/register', {username: username})
      .pipe(map(user => {
        // register succesful if there's a jwt token in response
        if (user && user.token) {
          // Store token in local storage to keep logged in between refreshes
          localStorage.setItem('currentUser', JSON.stringify(user));
        }

        return user;
      }));
  }
}

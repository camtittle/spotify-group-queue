import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterResponse } from '../models';

/*
 *  Intercepts http calls and adds the bearer token if one is stored in localStorage
 */
@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Add authorization header with JWT if available
    const currentUser = <RegisterResponse>JSON.parse(localStorage.getItem('currentUser'));
    if (currentUser && currentUser.authToken) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.authToken}`
        }
      });
    }

    return next.handle(request);
  }
}

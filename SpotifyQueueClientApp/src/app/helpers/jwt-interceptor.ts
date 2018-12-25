import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterResponse } from '../models';
import { AuthenticationService } from '../services';

/*
 *  Intercepts http calls and adds the bearer token if one is stored in localStorage
 */
@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  public constructor(private authService: AuthenticationService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Add authorization header with JWT if available
    const currentUser = this.authService.getAccessToken();
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

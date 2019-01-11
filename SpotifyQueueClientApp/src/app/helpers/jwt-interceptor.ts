import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccessToken } from '../models';
import { AuthenticationService } from '../services';
import { environment } from '../../environments/environment';

/*
 *  Intercepts http calls and adds the bearer token if one is stored in localStorage
 */
@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  private currentUser: AccessToken;

  public constructor(private authService: AuthenticationService) {
    authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Add authorization header with JWT if available
    if (request.url.includes(environment.baseApiUrl) && this.currentUser && this.currentUser.authToken) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${this.currentUser.authToken}`
        }
      });
    }

    return next.handle(request);
  }
}

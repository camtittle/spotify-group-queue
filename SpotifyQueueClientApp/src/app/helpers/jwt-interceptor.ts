import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { EMPTY, Observable } from 'rxjs';
import { AuthenticationService } from '../services';
import { environment } from '../../environments/environment';
import { SpotifyService } from '../services/spotify.service';
import { fromPromise } from 'rxjs/internal-compatibility';
import { mergeMap } from 'rxjs/operators';

/*
 *  Intercepts http calls and adds the bearer token if one is stored in localStorage
 */
@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  public constructor(private authService: AuthenticationService,
                     private spotifyService: SpotifyService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Add authorization header with JWT if available
    const currentUser = this.authService.currentUser$.getValue();
    if (request.url.includes(environment.baseApiUrl) && currentUser && currentUser.authToken) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.authToken}`
        }
      });
      return next.handle(request);
    } else if (request.url.includes(environment.spotify.baseApiUri) && this.spotifyService.authorized$.getValue()) {
      // Requests to Spotify API
      return fromPromise(this.spotifyService.getAccessToken()).pipe(
        mergeMap(token => {
          if (!token) {
            return EMPTY;
          }

          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`
            }
          });
          return next.handle(request);
        })
      );
    } else {
      return next.handle(request);
    }
  }
}

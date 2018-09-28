import { Injectable } from "@angular/core";
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

/* 
 *  Intercepts http calls and adds the bearer token if one is stored in localStorage
 */
@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // Add authorization header with JWT if available
        // let currentUser = JSON.parse(localStorage.getItem('currentUser'));
        // if (currentUser && currentUser.token) {
        //     request = request.clone({
        //         setHeaders: {
        //             Authorization: 'Bearer ${currentUser.token}'
        //         }
        //     });
        // }

        let token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiY2FtZXJvbiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcHJpbWFyeXNpZCI6IjA3ZmRhNjJlLTkyOGMtNGZmOS1iYTExLTFlYjI2NTk2NGYxZSIsImV4cCI6MTUzNDk3NTE2MywiaXNzIjoic3BvdGlmeXBhcnR5LmRldiIsImF1ZCI6InNwb3RpZnlwYXJ0eS5kZXYifQ.PcCUpWEbh914z8wadoQMmoBWnKQL78C6DSE4Gs3M8TM";

        request = request.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        });

        return next.handle(request);
    }
}

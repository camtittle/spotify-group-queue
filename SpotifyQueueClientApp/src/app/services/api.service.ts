import { catchError } from 'rxjs/operators';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private http: HttpClient,
              private router: Router) { }

  public get<T>(endpoint: string): Observable<T> {
    console.log('GET: ' + endpoint);
    return this.http.get<T>(this.getUrl(endpoint))
      .pipe(catchError(error => this.handleError(error)));
  }

  public post<T>(endpoint: string, body: any): Observable<T> {
    console.log('POST: ' + endpoint);
    return this.http.post<T>(this.getUrl(endpoint), body)
      .pipe(catchError(error => this.handleError(error)));
  }

  private getUrl(endpoint: string) {
    return environment.baseApiUrl + endpoint;
  }

  private handleError(error: HttpErrorResponse) {
    console.log(error);
    if (error.status === 401) {
      this.router.navigate(['']);
    }

    if (error.error instanceof ErrorEvent) {
      console.error('Exception occured: ', error.error.message);
    } else {
      console.error(`Server responded with ${error.status}`);
    }
    return throwError(error.error);
  }
}

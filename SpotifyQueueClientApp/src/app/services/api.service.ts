import { catchError } from 'rxjs/operators';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  public get<T>(endpoint: string): Observable<any> {
    console.log('GET: ' + endpoint);
    return this.http.get<T>(this.getUrl(endpoint))
      .pipe(catchError(this.handleError));
  }


  constructor(private http: HttpClient) { }

  public post<T>(endpoint: string, body: any): Observable<T> {
    console.log('POST: ' + endpoint);
    return this.http.post<T>(this.getUrl(endpoint), body)
      .pipe(catchError(this.handleError));
  }

  private getUrl(endpoint: string) {
    return environment.baseApiUrl + endpoint;
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      console.error('Exception occured: ', error.error.message);
    } else {
      console.error(`Server responded with ${error.status}`);
    }
    return throwError(error.error);
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppSettings } from '../app-settings';
import { Observable } from 'rxjs';


//model
export class User {
    id: string;
    username: string;
}

@Injectable ({
    providedIn: 'root'
})
export class UserService {

    constructor(private http: HttpClient) { }

    public getMe(): Observable<any> {
        console.log("getme");
        return this.http.get<any>(`${ AppSettings.BASE_API_URL }/users/me`);
    }

}
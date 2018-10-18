import { ApiService } from './api.service';
import { Injectable } from '@angular/core';
import { User } from '../models';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private apiService: ApiService) { }

  public getMe() {
    return this.apiService.get('/users/me');
  }

}

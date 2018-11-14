import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { Injectable } from '@angular/core';
import { Party } from '../models';
import {BehaviorSubject, Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PartyService {

  $currentParty = new BehaviorSubject<Party>(null);

  constructor(private apiService: ApiService) { }

  public create(name: string) {
    const body = {
      name: name
    };
    return this.apiService.post<Party>('/parties', body);
  }

  public async getAllParties(): Promise<Party[]> {
    return this.apiService.get<Party[]>('/parties/all').toPromise();
  }
}

import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { Injectable } from '@angular/core';
import { PartyListItem } from '../models';
import {BehaviorSubject, Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PartyService {

  $currentParty = new BehaviorSubject<PartyListItem>(null);

  constructor(private apiService: ApiService) { }

  public create(name: string) {
    const body = {
      name: name
    };
    return this.apiService.post<PartyListItem>('/parties', body);
  }

  public async getAllParties(): Promise<PartyListItem[]> {
    return this.apiService.get<PartyListItem[]>('/parties').toPromise();
  }
}

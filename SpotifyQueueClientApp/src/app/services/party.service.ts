import { ApiService } from './api.service';
import { Injectable } from '@angular/core';
import { PartyListItem } from '../models';
import { BehaviorSubject } from 'rxjs';
import { CurrentParty } from '../models/current-party.model';

@Injectable({
  providedIn: 'root'
})
export class PartyService {

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

  public async getCurrentParty(): Promise<CurrentParty> {
    return this.apiService.get<CurrentParty>('/parties/current').toPromise();
  }
}

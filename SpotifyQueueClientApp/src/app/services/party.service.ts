import { ApiService } from './api.service';
import { Injectable } from '@angular/core';
import { PartyListItem, CurrentParty } from '../models';

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

  public async requestToJoinParty(party: PartyListItem): Promise<void> {
    const body = {
      partyId: party.id
    };
    return this.apiService.post<void>('/parties/join', body).toPromise();
  }
}

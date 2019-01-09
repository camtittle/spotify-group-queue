import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CurrentParty } from '../models';
import { PartyHubService } from '../services/party-hub.service';
import { PartyService } from '../services';

@Injectable({
  providedIn: 'root'
})
export class PartyMembershipGuardGuard implements CanActivate {

  private currentParty: CurrentParty;

  constructor(private router: Router,
              private partyHubService: PartyHubService,
              private partyService: PartyService) {
    partyHubService.currentParty$.subscribe(x => this.currentParty = x);
  }

  async canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean> {

    if (this.currentParty) {
      return true;
    }

    // Double check with server that there is no party
    const party = await this.partyService.getCurrentParty();
    if (party) {
      this.partyHubService.updateCurrentParty(party);
      return true;
    }

    this.router.navigate(['find'], { queryParams: { returnUrl: state.url }});
  }
}

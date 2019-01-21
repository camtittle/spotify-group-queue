import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { BasePartyScreen } from '../base-party-screen';
import { PartyHubService } from '../../../services/party-hub.service';
import { AuthenticationService } from '../../../services';
import { SpotifyService } from '../../../services/spotify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-members',
  templateUrl: './members.component.html',
  styleUrls: ['./members.component.scss']
})
export class MembersComponent extends BasePartyScreen implements OnInit {

  constructor(protected partyHubService: PartyHubService,
              protected authService: AuthenticationService,
              protected spotifyService: SpotifyService,
              protected router: Router,
              protected cdRef: ChangeDetectorRef) {
    super(partyHubService, authService, spotifyService, router,  cdRef);
  }

  ngOnInit() {
  }

  protected onSpotifyAuthorization(authorized: boolean) {
  }

}

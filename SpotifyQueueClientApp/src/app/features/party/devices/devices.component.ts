import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { BasePartyScreen } from '../base-party-screen';
import { PartyHubService } from '../../../services/party-hub.service';
import { AuthenticationService } from '../../../services';
import { SpotifyService } from '../../../services/spotify.service';
import { SpotifyDevice } from '../../../models/spotify/spotify-device.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.scss']
})
export class DevicesComponent extends BasePartyScreen implements OnInit {

  public spotifyDevices: SpotifyDevice[];

  constructor(protected spotifyService: SpotifyService,
              protected partyHubService: PartyHubService,
              protected authService: AuthenticationService,
              protected router: Router,
              protected cdRef: ChangeDetectorRef) {
    super(partyHubService, authService, spotifyService, router, cdRef);
  }

  ngOnInit() {
  }

  protected async onSpotifyAuthorization(authorized: boolean) {
    console.log('spotify auth: ' + authorized);
    if (authorized && this.isOwner) {
      // do something
      this.spotifyDevices = await this.spotifyService.getDevices();
    }
  }

  public async onClickDeviceItem(device: SpotifyDevice) {
    await this.spotifyService.setPlaybackDevice(device);
  }

}

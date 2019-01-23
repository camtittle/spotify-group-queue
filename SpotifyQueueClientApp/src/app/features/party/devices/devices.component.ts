import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { BasePartyScreen } from '../base-party-screen';
import { PartyHubService } from '../../../services/party-hub.service';
import { AuthenticationService } from '../../../services';
import { SpotifyService } from '../../../services/spotify.service';
import { SpotifyDevice } from '../../../models/spotify/spotify-device.model';
import { Router } from '@angular/router';
import { fadeIn } from '../../../animations';
import { PlaybackStatusUpdate } from '../../../models/playback-status-update.model';

@Component({
  selector: 'app-devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.scss'],
  animations: [ fadeIn ]
})
export class DevicesComponent extends BasePartyScreen implements OnInit {

  public spotifyDevices: SpotifyDevice[];
  public activeDevice: SpotifyDevice;
  public loading = true;
  public spinner = false;
  private justSwitchedDevice = false;

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
    if (authorized && this.isOwner) {
      // Get latest devices
      await this.updateDeviceList();
      this.loading = false;
    }
  }

  public async onClickDeviceItem(device: SpotifyDevice) {
    await this.spotifyService.setPlaybackDevice(device);
    this.justSwitchedDevice = true;
  }

  private async updateDeviceList() {
    console.log('GET devices');
    this.spotifyDevices = await this.spotifyService.getDevices();

    const active = this.spotifyDevices.filter(x => x.is_active);
    if (active.length > 0) {
      this.activeDevice = active[0]

      const index = this.spotifyDevices.findIndex(x => x.id === this.activeDevice.id);
      this.spotifyDevices.splice(index, 1);

      await this.spotifyService.setPlaybackDevice(this.activeDevice);
    }

  }

  public async refresh() {
    console.log('refresh');
    this.loading = true;
    await this.updateDeviceList();
    this.loading = false;
    this.spinner = false;
  }

  protected async onPlaybackStatusUpdate(status: PlaybackStatusUpdate) {
    // if (this.justSwitchedDevice) {
    //   this.spinner = true;
    //   this.loading = true;
    //   setTimeout(async () => {
    //     await this.updateDeviceList();
    //     this.spinner = false;
    //     this.loading = false;
    //     this.justSwitchedDevice = false;
    //   }, 2000);
    // } else {
    //   await this.updateDeviceList();
    // }
  }

}

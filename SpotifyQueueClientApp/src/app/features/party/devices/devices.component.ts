import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { BasePartyScreen } from '../base-party-screen';
import { PartyHubService } from '../../../services/party-hub.service';
import { AuthenticationService } from '../../../services';
import { SpotifyService } from '../../../services/spotify.service';
import { SpotifyDevice } from '../../../models/spotify/spotify-device.model';
import { Router } from '@angular/router';
import { fadeIn } from '../../../animations';
@Component({
  selector: 'app-devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.scss'],
  animations: [ fadeIn ]
})
export class DevicesComponent extends BasePartyScreen implements OnInit {

  public spotifyDevices: SpotifyDevice[];
  public loadingDevice: SpotifyDevice;
  public loading = true;

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
    this.loadingDevice = device;
    const newDevice = await this.spotifyService.setPlaybackDevice(device);
    const oldActiveDevice = this.spotifyDevices.find(x => x.is_active);

    if (newDevice) {
      if (newDevice.deviceId !== device.id) {
        console.warn('ERROR --> Could not transfer to chosen device');
      } else {
        if (oldActiveDevice) {
          oldActiveDevice.is_active = false;
        }
        this.spotifyDevices.find(x => x.id === newDevice.deviceId).is_active = true;
      }
    } else {
      // this.loadingDevice = null;
      if (oldActiveDevice) {
        oldActiveDevice.is_active = false;
      }
    }
    this.loadingDevice = null;
  }

  private async updateDeviceList() {
    const devices = await this.spotifyService.getDevices();
    console.log(devices);
    const actives = devices.filter(x => x.is_active);

    if (actives.length > 0) {
      const active = actives[0];
      const index = devices.findIndex(x => x.id === active.id);
      devices.splice(index, 1);
      this.spotifyDevices = [active].concat(devices);

      await this.spotifyService.setPlaybackDevice(active);
    } else {
      this.spotifyDevices = devices;
      await this.spotifyService.setPlaybackDevice({id: null, name: null});
    }
  }

  public async refresh() {
    this.loading = true;
    await this.updateDeviceList();
    this.loading = false;
  }

}

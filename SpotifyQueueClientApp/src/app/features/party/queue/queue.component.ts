import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthenticationService, PartyService, SignalRConnectionService } from '../../../services/index';
import { BsModalService } from 'ngx-bootstrap';
import { PendingMemberRequestComponent } from '../../../modals/pending-member-request/pending-member-request.component';
import { AccessToken, PendingMemberRequest, CurrentParty, CurrentPartyQueueItem } from '../../../models/index';
import { ActivatedRoute, Router } from '@angular/router';
import { PartyHubService } from '../../../services/party-hub.service';
import { SpotifyService } from '../../../services/spotify.service';
import { SpotifyDevice } from '../../../models/spotify-device.model';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.scss']
})
export class QueueComponent implements OnInit, OnDestroy {

  public loading = true;

  public currentUser: AccessToken;
  public currentParty: CurrentParty;
  public authorizedWithSpotify: boolean;
  public spotifyDevices: SpotifyDevice[];

  constructor(private partyService: PartyService,
              private modalService: BsModalService,
              private authService: AuthenticationService,
              private partyHubService: PartyHubService,
              private spotifyService: SpotifyService,
              private route: ActivatedRoute,
              private router: Router) { }

  async ngOnInit() {
    this.authService.currentUser$.subscribe(user => this.currentUser = user);

    this.partyHubService.currentParty$.subscribe(party => this.currentParty = party);

    this.spotifyService.authorized$.subscribe(async authorized => {
      this.authorizedWithSpotify = authorized;

      if (authorized && this.isOwner()) {
        await this.loadSpotifyDevices();
      }
    });

    // Hub events
    this.partyHubService.observe<PendingMemberRequest>('onPendingMemberRequest').subscribe(request => {
      console.log('pending member request: ' + request.username);
      this.onPendingMemberRequest(request);
    });

    this.partyHubService.observe<boolean>('pendingMembershipResponse').subscribe(accepted => {
      this.onPendingMemberResponse(accepted);
    });

    this.loading = false;
  }

  ngOnDestroy() {

  }

  /**
   * Called on admin users when a user requests to join their party
   * @param request
   */
  private onPendingMemberRequest(request: PendingMemberRequest) {
    const initialState = {
      request: request
    };
    this.modalService.show(PendingMemberRequestComponent, {initialState});
  }

  /**
   * Called on a pending user when the admin accepts/declines their request to join
   * @param response
   */
  private onPendingMemberResponse(accepted: boolean) {
    if (accepted) {
      // Update current queue
      console.log('Admin ACCEPTED your request to join');
    } else {
      // Show rejection message
      console.log('Admin DECLINED your request to join');
    }
  }

  /**
   * Called on all users in a party when status of party changes
   * @param statusUpdate
   */
  private onPartyStatusUpdate(statusUpdate: CurrentParty) {
    console.log('party status update');
    this.currentParty = statusUpdate;
  }

  public async onClickAddTrack() {
    await this.router.navigate(['../search'], { relativeTo: this.route });
  }

  public onClickRemoveTrack(queueItem: CurrentPartyQueueItem) {
    this.partyHubService.invoke('removeTrackFromQueue', queueItem.id);
  }

  public onClickAuthorizeSpotify() {
    this.spotifyService.triggerAuthorizationFlow();
  }

  private async loadSpotifyDevices() {
    this.spotifyDevices = await this.spotifyService.getConnectDevices();
  }

  public async onClickDeviceItem(device: SpotifyDevice) {
    await this.spotifyService.setPlaybackDevice(device);
  }

  public isPendingMember(): boolean {
    if (this.currentParty && this.currentUser) {
      return this.currentParty.pendingMembers.findIndex(x => x.id === this.currentUser.id) >= 0;
    }
    return false;
  }

  public isMember(): boolean {
    if (this.currentParty && this.currentUser) {
      return this.currentParty.members.findIndex(x => x.id === this.currentUser.id) >= 0;
    }
    return false;
  }

  public isOwner(): boolean {
    if (this.currentParty && this.currentUser) {
      return this.currentParty.owner.id === this.currentUser.id;
    }
    return false;
  }

}
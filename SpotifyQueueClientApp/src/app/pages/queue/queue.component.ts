import { HubConnectionService } from './../../services/hub-connection.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthenticationService, PartyService } from '../../services';
import { CurrentParty } from '../../models/current-party.model';
import { BsModalService } from 'ngx-bootstrap';
import { PendingMemberRequest } from '../../models/pending-member-request.model';
import { PendingMemberRequestComponent } from '../../modals/pending-member-request/pending-member-request.component';
import { AccessToken } from '../../models';
import { QueueTrack } from '../../models/add-queue-track.model';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.css']
})
export class QueueComponent implements OnInit, OnDestroy {

  public loading = true;
  public message = '';

  public currentUser: AccessToken;
  public currentParty: CurrentParty;

  constructor(private hubConnectionService: HubConnectionService,
              private partyService: PartyService,
              private modalService: BsModalService,
              private authService: AuthenticationService) { }

  async ngOnInit() {
    await this.checkPartyMembership();
    if (!this.currentParty) {
      console.log('Error: not member of any party');
      this.loading = false;
      this.message = 'Not a member of a party';
      return;
    }

    // Access token subscription
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    // Establish connection to hub
    this.loading = true;
    console.log('loading');

    const pendingMemberRequest = await this.hubConnectionService.pendingMemberRequest$();
    pendingMemberRequest.subscribe(request => {
      console.log('pending member request: ' + request.username);
      this.onPendingMemberRequest(request);
    });

    const pendingMemberResponse = await this.hubConnectionService.pendingMemberResponse$();
    pendingMemberResponse.subscribe(accepted => {
      this.onPendingMemberResponse(accepted);
    });

    const partyStatusUpdate = await this.hubConnectionService.partyStatusUpdate$();
    partyStatusUpdate.subscribe(statusUpdate => {
      this.onPartyStatusUpdate(statusUpdate);
    })
    console.log('not loading');
    this.loading = false;
  }

  ngOnDestroy() {

  }

  private async checkPartyMembership() {
    this.currentParty = await this.partyService.getCurrentParty();
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

  /**
   * Demo implementation of adding track to queue
   * Currently adds a track with title "username:time" and artist "DJ username", 30s duration
   * TODO: Spotify implementation
   */
  public async onClickAddTrack() {
    console.log('add track to queue');
    if (!this.currentUser || !this.currentUser.username) {
      console.error('Cannot add track to queue - no current user');
      return;
    }

    const date = new Date();
    const track = <QueueTrack> {
      spotifyUri: 'a-made-up-uri',
      title: `${this.currentUser.username}:${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`,
      artist: `DJ ${this.currentUser.username}`,
      durationMillis: 30000
    };

    await this.hubConnectionService.addTrackToQueue(track);
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

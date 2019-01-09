import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthenticationService, PartyService, SignalRService } from '../../services';
import { BsModalService } from 'ngx-bootstrap';
import { PendingMemberRequestComponent } from '../../modals/pending-member-request/pending-member-request.component';
import { AccessToken, PendingMemberRequest, CurrentParty, CurrentPartyQueueItem } from '../../models';
import { Router } from '@angular/router';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.scss']
})
export class QueueComponent implements OnInit, OnDestroy {

  public loading = true;

  public currentUser: AccessToken;
  public currentParty: CurrentParty;

  constructor(private signalRService: SignalRService,
              private partyService: PartyService,
              private modalService: BsModalService,
              private authService: AuthenticationService,
              private router: Router) { }

  async ngOnInit() {
    await this.checkPartyMembership();
    if (!this.currentParty) {
      this.loading = false;
      this.currentParty = null;
      return;
    }

    // Current user subscription
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    // Hub events
    this.signalRService.observe<PendingMemberRequest>('onPendingMemberRequest').subscribe(request => {
      console.log('pending member request: ' + request.username);
      this.onPendingMemberRequest(request);
    });

    this.signalRService.observe<boolean>('pendingMembershipResponse').subscribe(accepted => {
      this.onPendingMemberResponse(accepted);
    });

    this.signalRService.observe<CurrentParty>('partyStatusUpdate').subscribe(statusUpdate => {
      this.onPartyStatusUpdate(statusUpdate);
    });

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

  public async onClickAddTrack() {
    await this.router.navigateByUrl('/search');
  }

  public onClickRemoveTrack(queueItem: CurrentPartyQueueItem) {
    this.signalRService.invoke('removeTrackFromQueue', queueItem.id);
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

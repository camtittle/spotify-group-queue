import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { AuthenticationService, PartyService } from '../../../services/index';
import { BsModalService } from 'ngx-bootstrap';
import { PendingMemberRequestComponent } from '../../../modals/pending-member-request/pending-member-request.component';
import { PendingMemberRequest, CurrentParty, CurrentPartyQueueItem, QueueTrack } from '../../../models/index';
import { ActivatedRoute, Router } from '@angular/router';
import { PartyHubService } from '../../../services/party-hub.service';
import { SpotifyService } from '../../../services/spotify.service';
import { BasePartyScreen } from '../base-party-screen';
import { fadeInOut } from '../../../animations';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.scss'],
  animations: [ fadeInOut ]
})
export class QueueComponent extends BasePartyScreen implements OnInit {

  public loading = true;

  constructor(protected authService: AuthenticationService,
              protected partyHubService: PartyHubService,
              protected spotifyService: SpotifyService,
              protected cdRef: ChangeDetectorRef,
              protected router: Router,
              private route: ActivatedRoute,
              private partyService: PartyService,
              private modalService: BsModalService) {
    super(partyHubService, authService, spotifyService, router, cdRef);
  }

  async ngOnInit() {
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

  public async onClickDebug() {
    // Trigger sequence of actions for debugging

    // Update playback
    console.log('Debug --> Getting playback status update')
    await this.partyHubService.getPlaybackStatusUpdate();

    setTimeout(async () => {
      const queueTrack = <QueueTrack> {
        spotifyUri: 'spotify:track:6SlZp9UeLRIuw92j96dcjU',
        title: 'Changing of The Seasons',
        artist: 'Two Door Cinema Club',
        durationMillis: 222880
      };

      console.log('Debug --> Adding queue item')
      await this.partyHubService.invoke('addTrackToQueue', queueTrack);
    }, 2000);


  }
}

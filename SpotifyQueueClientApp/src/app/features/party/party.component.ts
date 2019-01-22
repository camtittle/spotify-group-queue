import { Component, DoCheck, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { PartyHubService } from '../../services/party-hub.service';
import { SpotifyService } from '../../services/spotify.service';
import { AuthenticationService } from '../../services';
import { AccessToken, CurrentParty } from '../../models';
import { Playback, PlaybackStatusUpdate } from '../../models/playback-status-update.model';
import { Subscription } from 'rxjs';
import { SpotifyDevice } from '../../models/spotify/spotify-device.model';

@Component({
  selector: 'app-party',
  templateUrl: './party.component.html',
  styleUrls: ['./party.component.scss']
})
export class PartyComponent implements OnInit, OnDestroy, DoCheck {

  @ViewChild('playbackOverlay') playbackOverlay: ElementRef;

  public currentUser: AccessToken;
  public currentParty: CurrentParty;
  public playbackState: PlaybackStatusUpdate;
  public authorizedWithSpotify: boolean;
  public routerMarginBottom: number;

  private currentUserSub: Subscription;
  private currentPartySub: Subscription;
  private playbackStateSub: Subscription;
  private spotifySub: Subscription;

  // Make Playback enum visible in template
  public Playback = Playback;

  // partyHubService injected in order to ensure it is initialised for all child components
  constructor(private partyHubService: PartyHubService,
              private spotifyService: SpotifyService,
              private authService: AuthenticationService) { }

  ngOnInit() {
    this.currentUserSub = this.authService.currentUser$.subscribe(user => this.currentUser = user);

    this.currentPartySub = this.partyHubService.currentParty$.subscribe(party => this.currentParty = party);

    this.playbackStateSub = this.partyHubService.playbackState$.subscribe(state => {
      console.log('--> playback status update');
      console.log(state);
      this.playbackState = state;
    });

    this.spotifySub = this.spotifyService.authorized$.subscribe(async authorized => {
      this.authorizedWithSpotify = authorized;
    });
  }

  ngOnDestroy() {
    if (this.currentUserSub) {
      this.currentUserSub.unsubscribe();
    }

    if (this.currentPartySub) {
      this.currentPartySub.unsubscribe();
    }

    if (this.playbackStateSub) {
      this.playbackStateSub.unsubscribe();
    }

    if (this.spotifySub) {
      this.spotifySub.unsubscribe();
    }
  }

  ngDoCheck() {
    if (this.playbackOverlay) {
      this.routerMarginBottom = this.playbackOverlay.nativeElement.clientHeight;
    }
  }

  get isOwner(): boolean {
    return this.currentUser.id === this.currentParty.owner.id;
  }


}

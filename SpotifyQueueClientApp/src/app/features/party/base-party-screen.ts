import { AccessToken, CurrentParty } from '../../models';
import { PartyHubService } from '../../services/party-hub.service';
import { AfterViewInit, ChangeDetectorRef, ElementRef, OnChanges, OnDestroy, ViewChild } from '@angular/core';
import { AuthenticationService } from '../../services';
import { SpotifyService } from '../../services/spotify.service';
import { Router } from '@angular/router';
import { distinctUntilChanged } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { PlaybackStatusUpdate } from '../../models/playback-status-update.model';

export abstract class BasePartyScreen implements AfterViewInit, OnChanges, OnDestroy {

  @ViewChild('navigationBar',  {read: ElementRef}) navigationBar: ElementRef;

  public currentParty: CurrentParty;
  public currentUser: AccessToken;
  public playbackState: PlaybackStatusUpdate;
  public authorizedWithSpotify: boolean;
  public navigationBarHeight: number;

  private subscriptions: Subscription[] = [];

  public get isOwner(): boolean {
    if (this.currentParty && this.currentUser) {
      return this.currentParty.owner.id === this.currentUser.id;
    }
    return false;
  }

  public get isPendingMember(): boolean {
    if (this.currentParty && this.currentUser) {
      return this.currentParty.pendingMembers.findIndex(x => x.id === this.currentUser.id) >= 0;
    }
    return false;
  }

  public get isMember(): boolean {
    if (this.currentParty && this.currentUser) {
      return this.currentParty.members.findIndex(x => x.id === this.currentUser.id) >= 0;
    }
    return false;
  }

  protected onSpotifyAuthorization(authorized: boolean) {
    // Override in sub-class if required
  }

  protected async onPlaybackStatusUpdate(status: PlaybackStatusUpdate) {
    // Override in sub-class if required
  }

  protected constructor(protected partyHubService: PartyHubService,
                        protected authService: AuthenticationService,
                        protected spotifyService: SpotifyService,
                        protected router: Router,
                        protected cdRef: ChangeDetectorRef) {
    this.subscriptions.push(this.partyHubService.currentParty$.subscribe(party => {
      this.currentParty = party;
    }));

    this.subscriptions.push(this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    }));

    this.subscriptions.push(this.spotifyService.authorized$.pipe(distinctUntilChanged()).subscribe(async authorized => {
      this.authorizedWithSpotify = authorized;
      await this.onSpotifyAuthorization(authorized);
    }));

    this.subscriptions.push(this.partyHubService.playbackState$.subscribe(async state => {
      this.playbackState = state;
      await this.onPlaybackStatusUpdate(state);
    }));
  }

  ngAfterViewInit() {
    if (this.navigationBar && this.navigationBar.nativeElement) {
      this.navigationBarHeight = this.navigationBar.nativeElement.clientHeight + 8;
      this.cdRef.detectChanges();
    }
  }

  ngOnChanges() {
    if (this.navigationBar && this.navigationBar.nativeElement) {
      this.navigationBarHeight = this.navigationBar.nativeElement.clientHeight;
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}

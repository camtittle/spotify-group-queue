import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AuthenticationService } from '../../../services';
import { from, fromEvent, Observable } from 'rxjs';
import { distinctUntilChanged, filter, map, switchMap, tap, throttleTime } from 'rxjs/operators';
import { QueueTrack, TrackSearchResult } from '../../../models';
import { PartyHubService } from '../../../services/party-hub.service';
import { Router } from '@angular/router';
import { BasePartyScreen } from '../base-party-screen';
import { SpotifyService } from '../../../services/spotify.service';
import { Track } from '../../../models/track.model';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent extends BasePartyScreen implements OnInit, OnDestroy, AfterViewInit {

  @ViewChild('searchInput') searchInput: ElementRef;

  public searchInputValue: string;
  public results: Track[];

  private typeahead: Observable<Track[]>;

  constructor(protected authService: AuthenticationService,
              protected partyHubService: PartyHubService,
              protected spotifyService: SpotifyService,
              protected router: Router,
              protected cdRef: ChangeDetectorRef) {
    super(partyHubService, authService, spotifyService, router, cdRef);
  }

  async ngOnInit() {
    // Typeahead behavior
    this.typeahead = fromEvent<any>(this.searchInput.nativeElement, 'input').pipe(
      throttleTime(1000, undefined, {leading: true, trailing: true}),
      map(event => event.target.value),
      filter(query => query.length > 0),
      tap(query => console.log(query)),
      distinctUntilChanged(),
      switchMap(query => this.searchSpotify(query)),
    );

    this.typeahead.subscribe(result => {
      this.results = result;
    });
  }

  ngOnDestroy() {
  }

  ngAfterViewInit() {
    super.ngAfterViewInit();
    this.searchInput.nativeElement.focus();
  }

  protected onSpotifyAuthorization() {
  }

  private searchSpotify(query: string): Observable<Track[]> {
    return from<Track[]>(this.partyHubService.invoke('searchSpotifyTracks', query));
  }

  public async onTrackClick(track: Track) {
    if (!this.currentUser || !this.currentUser.username) {
      console.error('Cannot add track to queue - no current user');
      return;
    }

    const queueTrack = <QueueTrack> {
      spotifyUri: track.uri,
      title: track.title,
      artist: track.artist,
      durationMillis: track.durationMillis
    };

    await this.partyHubService.invoke('addTrackToQueue', queueTrack);
  }

}

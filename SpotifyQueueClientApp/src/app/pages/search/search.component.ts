import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AuthenticationService, HubConnectionService } from '../../services';
import { from, fromEvent, Observable } from 'rxjs';
import { distinctUntilChanged, filter, map, switchMap, tap, throttleTime } from 'rxjs/operators';
import { SpotifyTrack } from '../../models/spotify-track.model';
import { TrackSearchResult } from '../../models/track-search-result.model';
import { QueueTrack } from '../../models/add-queue-track.model';
import { AccessToken } from '../../models';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {

  @ViewChild('searchInput') searchInput: ElementRef;

  public searchInputValue: string;
  public results: SpotifyTrack[];
  private currentUser: AccessToken;
  private typeahead: Observable<SpotifyTrack[]>;

  constructor(private hubConnectionService: HubConnectionService,
              private authService: AuthenticationService) { }

  async ngOnInit() {
    // Typeahead behavior
    this.typeahead = fromEvent<any>(this.searchInput.nativeElement, 'input').pipe(
      throttleTime(1000, undefined, {leading: true, trailing: true}),
      map(event => event.target.value),
      filter(query => query.length > 0),
      tap(query => console.log(query)),
      distinctUntilChanged(),
      switchMap(query => this.searchSpotify(query))
    );

    this.typeahead.subscribe(result => {
      this.results = result;
      console.log(this.results);
    });

    // Access token subscription
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

  }

  private searchSpotify(query: string): Observable<SpotifyTrack[]> {
    return from<TrackSearchResult>(this.hubConnectionService.searchSpotify(query)).pipe(
      map(response => response.tracks.items)
    );
  }

  public async onTrackClick(track: SpotifyTrack) {
    console.log(track.name);
    if (!this.currentUser || !this.currentUser.username) {
      console.error('Cannot add track to queue - no current user');
      return;
    }

    const queueTrack = <QueueTrack> {
      spotifyUri: track.uri,
      title: track.name,
      artist: track.artists.map(x => x.name).join(', '),
      durationMillis: track.duration_ms
    };

    await this.hubConnectionService.addTrackToQueue(queueTrack);
  }



}

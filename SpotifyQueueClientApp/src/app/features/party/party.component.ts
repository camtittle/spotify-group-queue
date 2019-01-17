import { Component, OnInit } from '@angular/core';
import { PartyHubService } from '../../services/party-hub.service';
import { SpotifyService } from '../../services/spotify.service';
import { distinctUntilChanged } from 'rxjs/operators';
import { AuthenticationService } from '../../services';
import { AccessToken } from '../../models';
import { PlaybackState } from '../../models/spotify/spotify-playback-state.model';

@Component({
  selector: 'app-party',
  templateUrl: './party.component.html',
  styleUrls: ['./party.component.scss']
})
export class PartyComponent implements OnInit {

  public currentUser: AccessToken;

  public playbackState: PlaybackState;

  // partyHubService injected in order to ensure it is initialised for all child components
  constructor(private partyHubService: PartyHubService,
              private spotifyService: SpotifyService,
              private authService: AuthenticationService) { }

  ngOnInit() {

    this.spotifyService.authorized$.pipe(distinctUntilChanged()).subscribe(async authorized => {
      console.log('spotify authorized: ' + authorized);
    });

    this.authService.currentUser$.subscribe(user => this.currentUser = user);

    this.partyHubService.playbackState$.subscribe(state => {
      console.log('STATE MADE IT TO PARTY COMPONENT');
      console.log(state);
      this.playbackState = state;
    });
  }


}

import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { AccessToken, CurrentParty } from '../models';
import { environment } from '../../environments/environment';
import { SignalRConnectionService } from './signal-r-connection-service';
import { BehaviorSubject, Subject } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { PlaybackStatusUpdate } from '../models/playback-status-update.model';
import { SpotifyDevice } from '../models/spotify/spotify-device.model';
import { throttleTime } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PartyHubService {

  public currentParty$ = new BehaviorSubject<CurrentParty>(null);
  public playbackState$ = new BehaviorSubject<PlaybackStatusUpdate>(null);

  private currentUser: AccessToken;

  constructor(private apiService: ApiService,
              private authService: AuthenticationService,
              private signalRConnectionService: SignalRConnectionService) {

    authService.currentUser$.subscribe(user => this.currentUser = user);

    signalRConnectionService.setupConnection(environment.signalRHubUrl, () => this.getAccessToken());

    this.signalRConnectionService.connected$.subscribe(async connected => {
      if (connected) {
        const state = await this.signalRConnectionService.invoke<PlaybackStatusUpdate>('getCurrentPlaybackState');
        this.playbackState$.next(state);
      }
    });

    // Check if currently in a party before attempting hub connection
    this.getCurrentParty().then(party => {
      this.updateCurrentParty(party);
    });

    this.signalRConnectionService.observe<CurrentParty>('partyStatusUpdate').subscribe(party => {
      this.currentParty$.next(party);
    });

    this.signalRConnectionService.observe<PlaybackStatusUpdate>('playbackStatusUpdate').subscribe(state => {
      this.playbackState$.next(state);
    });
  }

  public updateCurrentParty(party: CurrentParty) {
    this.currentParty$.next(party);
    if (party) {
      this.signalRConnectionService.openConnection();
    }
  }

  private async getCurrentParty(): Promise<CurrentParty> {
    return this.apiService.get<CurrentParty>('/parties/current').toPromise();
  }

  // Wraps the SignalRService's invoke method
  public async invoke<T>(methodName: string, ...params: any[]): Promise<T> {
    return this.signalRConnectionService.invoke<T>(methodName, ...params);
  }

  // Wraps the SignalRService's observe method
  public observe<ArgType>(methodName: string): Subject<ArgType> {
    return this.signalRConnectionService.observe(methodName);
  }

  private getAccessToken(): string {
    if (this.currentUser) {
      return this.currentUser.authToken;
    }
    return null;
  }
}


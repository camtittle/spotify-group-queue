import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiService } from './api.service';
import { SpotifyAccessToken } from '../models/spotify/spotify-access-token.model';
import { HttpClient } from '@angular/common/http';
import { SpotifyDevice, SpotifyDevicesResponse } from '../models/spotify/spotify-device.model';
import { SetDeviceResponse } from '../models/set-device-response.model';
import { PartyHubService } from './party-hub.service';

@Injectable({
  providedIn: 'root'
})
export class SpotifyService {

  public authorized$ = new BehaviorSubject<boolean>(false);

  private accessToken: string;
  private expiry: Date;

  constructor(private httpClient: HttpClient,
              private apiService: ApiService,
              private partyHubService: PartyHubService) {
    console.log(apiService);
    this.checkAuthorizationStatus();
  }

  /**
   * Opens the Spotify authorization page to allow the user to connect their Spotify account
   */
  public triggerAuthorizationFlow() {
    const uri = this.getAuthorizationUri();
    window.location.href = uri;
  }

  /**
   * Compare received state from spotify callback with state stored in session storage
   * @param newState State value to compare with stored state
   */
  public validateState(newState: string): boolean {
    const savedState = sessionStorage.getItem('spotifyState');
    return savedState === newState;
  }

  /**
   * Validate authorization code with server and exchange for an Access Token
   * @param code Authorization code provided by Spotify during the auth flow
   */
  public async authorizeWithCode(code: string) {
    const token = await this.authorizeSpotifyWithServer(code);
    this.saveAccessToken(token);
  }

  /**
   * Returns a valid Spotify access token. Refreshes the stored token if necessary
   */
  public async getAccessToken(): Promise<string> {
    if (!this.authorized$.getValue()) {
      console.warn('Attempt to get a Spotify access token whilst in unauthorized state');
      return null;
    }

    const now = new Date();
    if (!this.accessToken || !this.expiry || this.expiry < now) {
      await this.refreshAccessToken();
    }

    return this.accessToken;
  }

  private async authorizeSpotifyWithServer(authCode: string): Promise<SpotifyAccessToken> {
    const response = await this.apiService.post<SpotifyAccessToken>('/spotify/authorize', {code: authCode}).toPromise();

    // Check required fields are present and save
    if (!this.accessTokenIsValid(response)) {
      return null;
    }

    return response;
  }

  // Ask our server for a new Spotify access token
  private async refreshAccessToken() {
    const token = await this.apiService.get<SpotifyAccessToken>('/spotify/refresh').toPromise();

    // Check required fields are present and save
    if (!this.accessTokenIsValid(token)) {
      return;
    }

    this.saveAccessToken(token);
  }

  private checkAuthorizationStatus() {
    const status = sessionStorage.getItem('authorizedSpotify');
    if (status && status === 'true') {
      this.authorized$.next(true);
    }
  }

  private accessTokenIsValid(token: SpotifyAccessToken): boolean {
    // Check required fields are present and save
    if (!(token && token.accessToken && token.expiresIn)) {
      // TODO: display a message to user - prompt to retry?
      this.authorized$.next(false);
      sessionStorage.removeItem('authorizedSpotify');
      console.warn('Missing fields in Spotify access token');
      return false;
    }

    return true;
  }

  private saveAccessToken(token: SpotifyAccessToken) {
    this.accessToken = token.accessToken;

    // Refresh 60 seconds early to prevent edge cases
    this.expiry = new Date();
    this.expiry.setSeconds(this.expiry.getSeconds() + token.expiresIn - 60);

    sessionStorage.setItem('authorizedSpotify', 'true');
    this.authorized$.next(true);
  }

  private getAuthorizationUri() {
    const env = environment.spotify;
    const queryParams = [
      'client_id=' + env.clientId,
      'response_type=code',
      'redirect_uri=' + env.redirectUri,
      'state=' + this.generateAndSaveState(),
      'scope=' + env.scopes.join(' '),
      'show_dialog=true'
    ];
    return encodeURI(env.authUri + '?' + queryParams.join('&'));
  }

  private generateAndSaveState() {
    const state = this.uuid();
    sessionStorage.setItem('spotifyState', state);
    return state;
  }

  private uuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      // tslint:disable-next-line
      const r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  public async getDevices(): Promise<SpotifyDevice[]> {
    console.log('GET devices');
    const response = await this.get<SpotifyDevicesResponse>('/me/player/devices');
    return response ? response.devices : [];
  }

  public async getPlaybackState(): Promise<any> {
    const response = await this.get<any>('/me/player');
    return response;
  }

  public async setPlaybackDevice(device: SpotifyDevice): Promise<SetDeviceResponse> {
    const body = {
      deviceName: device.name,
      deviceId: device.id
    };
    return await this.apiService.post<SetDeviceResponse>('/spotify/device', body).toPromise();
  }

  private async get<ReturnType>(endpoint: string): Promise<ReturnType> {
    const response = await this.httpClient.get<ReturnType>(this.getUrl(endpoint)).toPromise();
    return response;
  }

  private getUrl(endpoint: string) {
    return environment.spotify.baseApiUri + endpoint;
  }

  public async initializePlayback() {
    console.log(this.partyHubService);
    await this.partyHubService.invoke('activateQueuePlayback');
  }
}

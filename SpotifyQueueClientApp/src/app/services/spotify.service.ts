import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiService } from './api.service';
import { SpotifyAuthorizeResponse } from '../models/spotify-authorize-response.model';

@Injectable({
  providedIn: 'root'
})
export class SpotifyService {

  public connected$ = new BehaviorSubject<boolean>(false);

  constructor(private apiService: ApiService) { }

  public openAuthorizationDialog() {
    const uri = this.getAuthorizationUri();
    console.log(uri);
    window.location.href = uri;
  }

  /**
   * Compare received state from spotify callback with state stored in session storage
   * @param newState
   */
  public validateState(newState: string): boolean {
    const savedState = sessionStorage.getItem('spotifyState');
    return savedState === newState;
  }

  public async handleAuthCode(code: string) {
    console.log('code: ' + code);
    const response = await this.apiService.post<SpotifyAuthorizeResponse>('/spotify/authorize', {code: code}).toPromise();
    console.log('response:');
    console.log(response);
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

  private uuid(): string{
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      // tslint:disable-next-line
      const r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}

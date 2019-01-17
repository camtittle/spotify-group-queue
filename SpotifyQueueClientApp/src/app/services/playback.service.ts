import { Injectable } from '@angular/core';
import { SpotifyPlaybackState } from '../models/spotify/spotify-playback-state.model';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PlaybackService {

  public state$ = new BehaviorSubject<SpotifyPlaybackState>(null);

  constructor() { }
}

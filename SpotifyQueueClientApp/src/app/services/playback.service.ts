import { Injectable } from '@angular/core';
import { PlaybackStatusUpdate } from '../models/playback-status-update.model';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PlaybackService {

  public state$ = new BehaviorSubject<PlaybackStatusUpdate>(null);

  constructor() { }
}

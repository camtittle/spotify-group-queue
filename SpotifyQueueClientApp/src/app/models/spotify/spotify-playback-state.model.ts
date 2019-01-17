import { SpotifyDevice } from './spotify-device.model';
import { SpotifyTrack } from './spotify-track.model';

export interface SpotifyPlaybackState {
  device: SpotifyDevice;
  progress_ms: number;
  is_playing: boolean;
  item: SpotifyTrack;
}

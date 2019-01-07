import { SpotifyTrack } from './spotify-track.model';

export interface TrackSearchResult {
  tracks: PagingObject<SpotifyTrack>;
}

export interface PagingObject<T> {
  items: T[];
  limit: number;
  next: string;
  offset: number;
  previous: string;
  total: number;
}

export interface SpotifyTrack {
  uri: string;
  artists: SpotifyArtist[];
  name: string;
  duration_ms: number;
}

export interface SpotifyArtist {
  name: string;
  uri: string;
}

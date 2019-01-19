export interface PlaybackStatusUpdate {
  uri: string;
  title: string;
  artist: string;
  durationMillis: number;
  isPlaying: boolean;

  // Admin fields
  playbackState: Playback;
  deviceId: string;
  deviceName: string;
}

export enum Playback {
  NOT_ACTIVE = 0,
  PLAYING = 1,
  PAUSED = 2
}

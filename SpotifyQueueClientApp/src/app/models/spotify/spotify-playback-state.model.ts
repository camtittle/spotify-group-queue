export interface PlaybackState {
  uri: string;
  title: string;
  artist: string;
  durationMillis: number;
  isPlaying: boolean;

  deviceId: string;
  deviceName: string;
}

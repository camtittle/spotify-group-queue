export interface SpotifyDevicesResponse {
  devices: SpotifyDevice[];
}

export interface SpotifyDevice {
  id: string;
  is_active: boolean;
  is_restricted: boolean;
  name: string;
  type: string;
  volume_percent: number;
}

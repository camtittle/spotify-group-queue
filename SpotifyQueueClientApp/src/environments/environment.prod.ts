import { secrets } from './secrets';

export const environment = {
  // TODO change this to false!!!
  production: false,
  useDevRegisterEndpoint: true,
  baseApiUrl: 'http://localhost:58953/api/v1',
  signalRHubUrl: 'http://localhost:58953/partyHub',
  devPassword: secrets.DEV_PASSWORD,
  spotify: {
    clientId: '17607b88c036478e93260208af2017bd',
    baseApiUri: 'https://api.spotify.com/v1',
    authUri: 'https://accounts.spotify.com/authorize',
    redirectUri: 'http://localhost:4200/spotifycallback',
    scopes: [
      'playlist-read-private',
      'user-modify-playback-state',
      'user-read-currently-playing',
      'user-read-playback-state,',
      'user-library-read'
    ]
  }
};

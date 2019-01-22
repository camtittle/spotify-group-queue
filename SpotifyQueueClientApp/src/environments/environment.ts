// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import {secrets} from './secrets';

export const environment = {
  production: false,
  appName: 'AppName',
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

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.

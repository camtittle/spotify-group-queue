## Concept:
One user creates a Party, and other users can then join the Party. Members of the party can search in app for songs on Spotify and add them to the queue. The admin connects to their Spotify account and is able to select any of their Spotify Connect devices for playback - by default playback will continue on the current playback device. THe queue will update across all clients in real time, so everyone can see the current state of the queue.

The app is currently in a proof of concept phase (i.e. its very ugly)

### Features
(Not necessarily implemented yet)
* Create a username, create a Party or join other's parties
* Search for songs from Spotify catalogue and add them to the queue. Everyone can see the queue in real time.
* Admin can connect to Spotify, choose a playback device and start the queue playback. 
* Admin can control playback play, pause, skip etc.
* Admin can remove songs from the queue and reorder the queue.

#### Nice to have's
* Other user's can connect to Spotify in order to see their playlists and/or Spotify library in order to find songs to queue up
* Users can swap the position of 2 songs they have added to the queue

## Implementation
Client is Angular 6, server is .NET Core 2.1. I'm using SignalR for a websocket connection to the server for the real time stuff. The server is hooked up to Spotify to allow users to search for songs without logging in to Spotify, and the admin auths with spotify to allow the server to control playback on their Spotify account. 

### Queueing mechanism
Unfortunately Spotify's web API does not allow adding to the queue, so we have to do a bit of a workaround. Current plan is to request the song to be played, and then start a timer (effectively) to push the next song to Spotify right when the current one finishes. This is yet to be implemented so it will be interesting to see how well this works. If there are a few songs on the queue already, we could potentially send them all to Spotify as a context, and it will then work through them. If queue items are reordered or removed though we will need to replace the context when the current song ends - but this would allow for smoother playback.

## Set-up
### Server
Database is using EF, so just run `Update-Database` to intialise the DB. For the server, appsettings.json contains most of the config such as the database connection string, and Spotify Client ID. You will need to set up a project in the Spotify developer console and get the Client ID and secret. Security stuff is in the .NET User Secrets file - there is a `secrets-example.json` in the root. Use `cat ./secrets.json | dotnet user-secrets set` to set the secrets, or Manage User Secrets in Visual Studio.

### Client
Config is in `environment.ts`. Set the Api and SignalR Hub URI's appropriately - the API lives at `/api/v1` and the hub at `/partyHub`. The `useDevRegisterEndpoint` when set to `true` will allow you to log in as any existing user by typing their username on the login screen - this is handy because there is no concept of a persistent account with a password - when a user normally creates a username, an 'account' is created, and they will have an access token but there is no way to 'log in' as that user again without creating a new 'user'. To use this you need to set a `DEV_PASSWORD` in `secrets.ts`. Again there is an example file. THe dev password should be set in the server secrets also.

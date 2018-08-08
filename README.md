## Concept:
One user creates a "Party", and is then the admin of the party. Other users can then join the party from their own devices via a browser. To avoid using unique codes which need to be shared, could perhaps use one of the following techniues:
- Request location permission from user (possible in all modern browsers), then find nearby Parties, list them, then user can request to join one. Admin gets notified and can accept/decline.
- Use client's extenal IP to identify connections from the same network and find Parties coming from the same network. Cons: doens't work on cellular, may encounter problems with networks using their own assigned subnets (I'm thinking universities (including halls), probably even large apartment complexes etc), but would work well for home networks.
- Use facebook authentication (quick process for most users since they will already have facebook cookies), then list Parties being hosted by facebook friends. Cons: Not everyone at the party will be facebook friends.

Location based seems like best idea to me, but can have a fallback code/party name (set by the admin?).

### Client functionality
Once granted access, clients can add songs to the communal queue. All songs in the communal queue will be displayed to all clients, and updated live as songs are added (or removed). Clients can remove songs from the queue that they added, move a song down in the queue, or swap two of their songs positions'. Potentially could have a limit on the number of songs a client can have in the queue at any one time to prevent spamming? Ideally there will be no login process for clients, and they will be authenticated only by a time-limited session cookie (ie each browser will act as an independent client).

### Admin functionality
The admin client can close the party. When creating a Party, the admin must connect their spotify account (must be spotify premium). Admin can select which of their Spotify Connect devices to use for playback. Admins can remove songs from the queue, move songs positions in the queue, and kick users from the party. Admins can invite other clients in the rooms to be co-admins. Co-admins cannot close the Party or kick any other admins or co-admins. Admins and co-admins can pause/resume playback and skip songs. The Admin can immediately interrupt the currently playing song with a song of their choice, after which playback will return to the queue.


## Implementation
Will use the Spotify Web API, Angular on the clients and .NET Web API on the server. This could all probably be done without any server stuff, using WebSockets and keeping everything in the Admin client's local storage, but would have to deal with Admin losing connection/ leaving/ closing browser, and re-syncing after they return etc.

### Server
Server will store information about each Party including:
- ID, Name, AdminID/Owner, CurrentPlaybackDevice, CurrentSongURI, Queue (List of URIs?), (WIP)
(Might want to use a noSQL DB for this since we are gonna want to be able to modify the queue and reorder and stuff... not sure yet. If doing it relational then we could have a QueueItems table with Party link and a QueuePosition entry, and remember to delete any entries in the table after they have been played/party is closed. But QueueEntry will need to be updated whenever anything is moved in the queue... This needs some thought still.)

Server will store information about each client including:
- SessionID (cookie token), Privilege, CurrentParty, SongCount (num songs they have in queue), (WIP)

### Queueing mechanism
Spotify web API doesn't alow you to put songs on to a queue so we'll have to be a bit creative. Potential solution is to keep a list of the songs in the queue server-side. I need to experiment with the API a bit to see whether the playback requests can be done from our server. The authentication flow is such that the server will get an auth token which can be used to send requests to the Web API to monitor and change playback. This means the Admin won't have to keep their browser open (especially a problem if using a phone). We COULD compile a playlist of the queue URIs and send to spotify as a playback context: the spotify player will then work through the songs. Then changes to the queue will update that playlist. The playlist will actually be in the Admin's spotify account, and will be deleted when the Party is closed/expires. This will avoid having to snoop the current playback state, BUT will need to experiment to see whether changes to the playlist are reflected automatically in the playback context.
Alternatively, could only provide spotify with the current song to play, then either a) note the length of the song and wait that amount of time, OR poll currentlyPlaying endpoint to see how long is left on playback of current song. When at the end of the song, fire off a request to play the next song. Not as elegant, could result in glitchy playback and/or gaps in playback. If want to support skipping in spotify app as well, would need to do playback snooping. Snooping would put additional load on our server, so really isn't ideal.
When a client adds a song to the queue we can simply notify the server which will update its internal represetation of the current queue.

### Client UI
By default, a client will be provided with the ability to see the current queue, and to search the spotify catalogue for a song they want to play. Search will search by artist/title/album and can be done without connecting to a spotify account. Optionally, a client can log in to spotify (connect to spotify account), then they will be presented with their own playlists (and perhaps saved library) from which they can selct songs to play also. Once they have added songs to the playlist they will have options to move (click + drag?) songs DOWN the queue, or swap two of their songs in the queue (click one, click another, click swap button?). We are gonna want the queue status to be as live as possible, so will look into using Web Sockets to avoid excessive polling of the server.

### Admin UI
Same as client UI after spotify log in, plus an "admin" screen with list of users and ability to kick, promote. An option to select the current Spotify Connect playback device. Admin will be able to modify the queue at will (move songs UP/DOWN, remove songs). When adding song to the queue, perhaps in the "Added to queue successfully" toast we can have a "Or play now" option to play that song immediately.


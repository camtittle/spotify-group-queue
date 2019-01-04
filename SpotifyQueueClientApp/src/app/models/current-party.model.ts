import { User } from './user.model';

export interface CurrentParty {
  id: string;
  name: string;
  owner: User;
  members: User[];
  pendingMembers: User[];
  queueItems: CurrentPartyQueueItem[];
}

export interface CurrentPartyQueueItem {
  id: string;
  username: string;
  title: string;
  artist: string;
  spotifyUri: string;
  durationMillis: string;
}

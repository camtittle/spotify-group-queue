import { User } from './user.model';

export interface CurrentParty {
  id: string;
  name: string;
  owner: User;
  members: User[];
  pendingMembers: User[];
}

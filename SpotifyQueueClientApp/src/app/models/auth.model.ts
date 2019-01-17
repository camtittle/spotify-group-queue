import { CurrentParty } from './current-party.model';

export interface AccessToken {
  id: string;
  username: string;
  isOwner: boolean;
  authToken: string;
  currentParty: CurrentParty;
}

import { CurrentParty } from './current-party.model';

export interface AccessToken {
  id: string;
  username: string;
  authToken: string;
  currentParty: CurrentParty;
}

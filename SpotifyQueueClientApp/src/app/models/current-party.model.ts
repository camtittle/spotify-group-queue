export interface CurrentParty {
  id: string;
  name: string;
  owner: string;
  members: string[];
  membershipStatus: PartyMembershipStatus;
}

export enum PartyMembershipStatus {
  owner = 0,
  member = 1,
  pendingMember = 2,
  none = 3
}

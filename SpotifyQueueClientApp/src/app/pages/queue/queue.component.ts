import { HubConnectionService } from './../../services/hub-connection.service';
import { Component, OnInit } from '@angular/core';
import { PartyService } from '../../services';
import { CurrentParty } from '../../models/current-party.model';
import { BsModalService } from 'ngx-bootstrap';
import { PendingMemberRequest } from '../../models/pending-member-request.model';
import { PendingMemberRequestComponent } from '../../modals/pending-member-request/pending-member-request.component';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.css']
})
export class QueueComponent implements OnInit {

  public loading = true;
  public message = '';

  public currentParty: CurrentParty;

  // TODO remove this
  public pendingMember: string;

  constructor(private hubConnectionService: HubConnectionService,
              private partyService: PartyService,
              private modalService: BsModalService) { }

  async ngOnInit() {
    await this.checkPartyMembership();
    if (!this.currentParty) {
      console.log('Error: not member of any party');
      this.loading = false;
      this.message = 'Not a member of a party';
      return;
    }

    // Establish connection to hub
    this.loading = true;
    console.log('loading');

    const pendingMemberRequest = await this.hubConnectionService.pendingMemberRequest$();
    pendingMemberRequest.subscribe(request => {
      console.log('pending member request: ' + request.username);
      this.onPendingMemberRequest(request);
    });
    console.log('not loading');
    this.loading = false;

  }

  private async checkPartyMembership() {
    this.currentParty = await this.partyService.getCurrentParty();
  }

  private onPendingMemberRequest(request: PendingMemberRequest) {
    const initialState = {
      request: request
    };
    this.modalService.show(PendingMemberRequestComponent, {initialState});
  }

}

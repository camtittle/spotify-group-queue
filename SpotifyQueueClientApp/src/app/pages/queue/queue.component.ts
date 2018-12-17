import { HubConnectionService } from './../../services/hub-connection.service';
import { Component, OnInit } from '@angular/core';
import { PartyService } from '../../services';
import { CurrentParty } from '../../models/current-party.model';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.css']
})
export class QueueComponent implements OnInit {

  public loading = true;
  public message = '';

  public currentParty: CurrentParty;

  constructor(private hubConnectionService: HubConnectionService,
              private partyService: PartyService) { }

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
    const receiveMessage = await this.hubConnectionService.receiveMessage$();
    receiveMessage.subscribe(data => {
      this.message = data.message;
    });
    console.log('not loading');
    this.loading = false;

  }

  private async checkPartyMembership() {
    this.currentParty = await this.partyService.getCurrentParty();
  }


}

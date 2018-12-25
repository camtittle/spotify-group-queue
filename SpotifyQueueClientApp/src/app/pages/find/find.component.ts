import { Component, OnInit } from '@angular/core';
import {PartyListItem} from '../../models';
import {PartyService} from '../../services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-find',
  templateUrl: './find.component.html',
  styleUrls: ['./find.component.css']
})
export class FindComponent implements OnInit {

  public parties: PartyListItem[];

  constructor(private partyService: PartyService,
              private router: Router) { }

  async ngOnInit() {
    this.parties = await this.partyService.getAllParties();
  }

  public async joinParty(party: PartyListItem) {
    await this.partyService.requestToJoinParty(party);
    await this.router.navigateByUrl('/queue');
  }

}

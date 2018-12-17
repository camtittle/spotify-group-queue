import { Component, OnInit } from '@angular/core';
import {PartyListItem} from '../../models';
import {PartyService} from '../../services';

@Component({
  selector: 'app-find',
  templateUrl: './find.component.html',
  styleUrls: ['./find.component.css']
})
export class FindComponent implements OnInit {

  public parties: PartyListItem[];

  constructor(private partyService: PartyService) { }

  async ngOnInit() {
    this.parties = await this.partyService.getAllParties();
  }

}

import { Component, OnInit } from '@angular/core';
import {Party} from '../../models';
import {PartyService} from '../../services';

@Component({
  selector: 'app-find',
  templateUrl: './find.component.html',
  styleUrls: ['./find.component.css']
})
export class FindComponent implements OnInit {

  public parties: Party[];

  constructor(private partyService: PartyService) { }

  async ngOnInit() {
    this.parties = await this.partyService.getAllParties();
  }

}

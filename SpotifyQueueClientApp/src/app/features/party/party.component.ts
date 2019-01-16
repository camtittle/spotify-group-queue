import { Component, OnInit } from '@angular/core';
import { PartyHubService } from '../../services/party-hub.service';

@Component({
  selector: 'app-party',
  templateUrl: './party.component.html',
  styleUrls: ['./party.component.scss']
})
export class PartyComponent implements OnInit {

  // partyHubService injected in order to ensure it is initialised for all child components
  constructor(private partyHubService: PartyHubService) { }

  ngOnInit() {
  }

}

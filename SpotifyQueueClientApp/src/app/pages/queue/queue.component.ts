import { Subject } from 'rxjs';
import { HubConnectionService } from './../../services/hub-connection.service';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.css']
})
export class QueueComponent implements OnInit {

  public loading = true;
  public message = '';

  constructor(private hubConnectionService: HubConnectionService) { }

  async ngOnInit() {
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


}

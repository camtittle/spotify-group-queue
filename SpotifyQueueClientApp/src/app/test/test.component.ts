import { Component, OnInit } from '@angular/core';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr'

@Component({
  selector: 'app-test',
  templateUrl: './test.component.html',
  styleUrls: ['./test.component.css']
})
export class TestComponent implements OnInit {

  private hubConnection: HubConnection;
  constructor() { }

  ngOnInit() {
    this.hubConnection = new signalR.HubConnectionBuilder().withUrl("http://localhost:58953/partyhub").build();

    this.hubConnection.on("receiveMessage", (user, message) => {
      console.log(user + ": " + message);
    });

    this.hubConnection.start()
      //.then( () => this.hubConnection.invoke("receiveMessage", "Hello"))
      .catch( err => {
      return console.error(err.toString());
      }
    );



  }

}

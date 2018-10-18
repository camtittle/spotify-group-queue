import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { environment } from '../../environments/environment';
import { Subject } from 'rxjs';

/*
 * Manages a singleton SignalR Hub connection
 * Wrapping hub events with Observables
 */
@Injectable({
  providedIn: 'root'
})
export class HubConnectionService implements OnDestroy {

  private _connection: HubConnection;

  // Connection closed observable, emits the error that caused disconnection (if there is one)
  public connectionClosed$ = new Subject<any>();

  public receieveMessage$ = new Subject<{user: string, message: string}>();

  constructor() { }

  public get connection() {
    if (!this._connection) {
      this.initConnection();
    }
    return this._connection;
  }

  public ngOnDestroy() {
    if (this._connection) {
      this._connection.stop();
    }
  }

  public closeConnection() {
    if (this._connection) {
      this._connection.stop();
      this._connection = null;
    }
  }

  private async initConnection() {
    this._connection = new HubConnectionBuilder()
      .withUrl(environment.SIGNALR_HUB_URL)
      .build();

    this._connection.onclose(err => this.connectionClosed$.next(err));

    // Listen for hub events
    this._connection.on('receiveMessage', (user, message) => {
      console.log('yo');
      this.receieveMessage$.next({user, message});
    });

    await this._connection.start();
  }
}

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
  private _connectionClosed$: Subject<any>;
  private _receiveMessage$: Subject<{user: string, message: string}>;

  constructor() { }

  public ngOnDestroy() {
    if (this._connection) {
      this._connection.stop();
    }
  }

  private async getConnection(): Promise<HubConnection> {
    if (!this._connection) {
      await this.initConnection();
    }
    return this._connection;
  }

  public closeConnection() {
    if (this._connection) {
      this._connection.stop();
      this._connection = null;
    }
  }

  private async initConnection() {
    this._connection = new HubConnectionBuilder()
      .withUrl(environment.signalRHubUrl)
      .build();

    this._connection.onclose(err => {
      this._connectionClosed$.next(err);
      this._connection = null;
    });

    console.log('starting connection');
    await this._connection.start();
    console.log('connection started');
  }

  public async receiveMessage$() {
    if (!this._receiveMessage$) {
      this._receiveMessage$ = new Subject<{user: string, message: string}>();
      const conn = await this.getConnection();
      conn.on('receiveMessage', (user, message) => {
        this._receiveMessage$.next({user, message});
      });
    }
    return this._receiveMessage$;
  }
}
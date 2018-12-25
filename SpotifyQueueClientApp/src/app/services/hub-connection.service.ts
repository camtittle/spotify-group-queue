import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { environment } from '../../environments/environment';
import { Subject } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { PendingMemberRequest } from '../models/pending-member-request.model';

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
  private _pendingMemberRequest$: Subject<PendingMemberRequest>;
  private _pendingMemberResponse$: Subject<boolean>;

  constructor(private authenticationService: AuthenticationService) { }

  public async ngOnDestroy() {
    if (this._connection) {
      await this._connection.stop();
    }
  }

  private async getConnection(): Promise<HubConnection> {
    if (!this._connection) {
      await this.initConnection();
    }
    return this._connection;
  }

  public async closeConnection() {
    if (this._connection) {
      await this._connection.stop();
      this._connection = null;
    }
  }

  private async initConnection() {
    this._connection = new HubConnectionBuilder()
      .withUrl(environment.signalRHubUrl, {accessTokenFactory: () => this.getAccessToken()})
      .build();

    this._connection.onclose(err => {
      this._connectionClosed$.next(err);
      this._connection = null;
    });

    console.log('starting connection');
    await this._connection.start();
    console.log('connection started');
  }

  public async pendingMemberRequest$(): Promise<Subject<PendingMemberRequest>> {
    if (!this._pendingMemberRequest$) {
      this._pendingMemberRequest$ = new Subject<PendingMemberRequest>();
      const conn = await this.getConnection();
      conn.on('onPendingMemberRequest', (user: PendingMemberRequest) => {
        console.log(user);
        this._pendingMemberRequest$.next(user);
      });
    }
    return this._pendingMemberRequest$;
  }

  public async pendingMemberResponse$(): Promise<Subject<boolean>> {
    if (!this._pendingMemberResponse$) {
      this._pendingMemberResponse$ = new Subject<boolean>();
      const conn = await this.getConnection();
      console.log('--> pendingMemberResponse initialised');
      conn.on('pendingMembershipResponse', (accepted: boolean) => {
        console.log('response');
        console.log(accepted);
        this._pendingMemberResponse$.next(accepted);
      });
    }
    return this._pendingMemberResponse$;
  }

  public async acceptPendingMember(pendingUserId: string, accept: boolean) {
    const conn = await this.getConnection();
    await conn.invoke('acceptPendingMember', pendingUserId, accept);
  }


  private getAccessToken(): string {
    const token = this.authenticationService.getAccessToken();
    return token.authToken;
  }
}

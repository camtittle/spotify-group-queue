import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { environment } from '../../environments/environment';
import { Subject } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { PendingMemberRequest } from '../models/pending-member-request.model';
import { AccessToken } from '../models';
import { CurrentParty } from '../models/current-party.model';
import { QueueTrack } from '../models/add-queue-track.model';

/*
 * Manages a singleton SignalR Hub connection
 * Wrapping hub events with Observables
 */
@Injectable({
  providedIn: 'root'
})
export class HubConnectionService implements OnDestroy {

  private currentUser: AccessToken;

  private _connection: HubConnection;
  private _connectionClosed$: Subject<any>;

  // Observables wrapping hub events
  private _pendingMemberRequest$: Subject<PendingMemberRequest>;
  private _pendingMemberResponse$: Subject<boolean>;
  private _partyStatusUpdate$: Subject<CurrentParty>;

  constructor(private authService: AuthenticationService) {
    authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

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

  /**
   * Admin clients receive incoming pending membership requests
   */
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

  /**
   * Pending membership request was accepted/declined
   */
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

  public async partyStatusUpdate$(): Promise<Subject<CurrentParty>> {
    if (!this._partyStatusUpdate$) {
      this._partyStatusUpdate$ = new Subject<CurrentParty>();
      const conn = await this.getConnection();
      console.log('init party status update');
      conn.on('partyStatusUpdate', (partyStatus: CurrentParty) => {
        console.log('party status');
        this._partyStatusUpdate$.next(partyStatus);
      });
    }
    return this._partyStatusUpdate$;
  }

  /**
   * Calls method in Hub to accept/decline a pending membership request
   * @param pendingUserId
   * @param accept
   */
  public async acceptPendingMember(pendingUserId: string, accept: boolean) {
    const conn = await this.getConnection();
    await conn.invoke('acceptPendingMember', pendingUserId, accept);
  }

  /**
   * Calls method in Hub to add the given track to the current user's party's queue
   * @param track
   */
  public async addTrackToQueue(track: QueueTrack) {
    console.log('conn hub --> add track to queue');
    const conn = await this.getConnection();
    await conn.invoke('addTrackToQueue', track);
  }

  private getAccessToken(): string {
    if (this.currentUser) {
      return this.currentUser.authToken;
    }
    return null;
  }
}

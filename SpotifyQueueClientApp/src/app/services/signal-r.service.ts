import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { environment } from '../../environments/environment';
import { BehaviorSubject, Subject } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { AccessToken } from '../models';

/*
 * Manages a singleton SignalR Hub connection
 * Wrapping hub events with Observables
 */
@Injectable({
  providedIn: 'root'
})
export class SignalRService implements OnDestroy {

  public connected$ = new BehaviorSubject<boolean>(false);

  private currentUser: AccessToken;
  private connection: HubConnection;

  // Observables wrapping hub events
  private hubEvents: { [methodName: string]: Subject<any> } = {};

  constructor(private authService: AuthenticationService) {
    this.setupConnection();

    this.connection.start().then(() => {
      console.log('--> SIGNALR CONNECTION STARTED');
      this.connected$.next(true);
    });

    authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  public ngOnDestroy() {
    if (this.connection) {
      this.connection.stop(); // Ignored promise
    }
  }

  private setupConnection() {
    this.connection = new HubConnectionBuilder()
      .withUrl(environment.signalRHubUrl, {accessTokenFactory: () => this.getAccessToken()})
      .build();

    this.connection.onclose(err => {
      this.connected$.next(false);
      this.connection = null;
    });
  }

  public async closeConnection() {
    if (this.connection) {
      await this.connection.stop();
    }
    this.connected$.next(false);
  }

  /**
   * Get an observable for calls to the given client side method name.
   * @param methodName
   */
  public observe<ArgType>(methodName: string): Subject<ArgType> {
    // If there is already a listener for this method name, return that
    if (!this.hubEvents[methodName]) {
      this.hubEvents[methodName] = new Subject<ArgType>();

      // Add hub listener that pushes the result to the observable
      this.connection.on(methodName, (arg: ArgType) => {
        this.hubEvents[methodName].next(arg);
      });
    }

    return this.hubEvents[methodName];
  }

  /**
   * Invoke a method on the connected hub, ensuring the connection is alive first
   * @param methodName
   * @param params
   */
  public async invoke<T>(methodName: string, ...params: any[]): Promise<T> {
    // TODO: do we need this await?
    return await this.connection.invoke(methodName, ...params);
  }

  private getAccessToken(): string {
    if (this.currentUser) {
      return this.currentUser.authToken;
    }
    return null;
  }
}

import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@aspnet/signalr';
import { BehaviorSubject, Subject } from 'rxjs';
import { Injectable } from '@angular/core';

/*
 * Manages a singleton SignalR Hub connection
 * Wrapping hub events with Observables
 */
@Injectable({
  providedIn: 'root'
})
export class SignalRConnectionService {

  public connected$ = new BehaviorSubject<boolean>(false);

  private connection: HubConnection;
  private connecting = false;

  // Observables wrapping hub events
  private hubEvents: { [methodName: string]: Subject<any> } = {};

  constructor() {
  }

  public async openConnection() {
    if (this.connection.state !== HubConnectionState.Connected && !this.connecting) {
      console.log('Establishing hub connection');
      this.connecting = true;
      await this.connection.start();
      this.connected$.next(true);
    }

    this.connecting = false;
    console.log('Hub connection established');
  }

  public async closeConnection() {
    if (this.connection) {
      await this.connection.stop();
    }
    this.connected$.next(false);
  }

  public setupConnection(url: string, accessTokenProvider: () => string) {
    if (this.connection) {
      return;
    }

    this.connection = new HubConnectionBuilder()
      .withUrl(url, {accessTokenFactory: () => accessTokenProvider()})
      .build();

    this.connection.onclose(err => {
      this.connected$.next(false);
      this.connection = null;
    });
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
    if (this.connection.state !== HubConnectionState.Connected) {
      console.warn('Cannot invoke method ' + methodName + ' - not connected to hub');
      return;
    }
    return await this.connection.invoke<T>(methodName, ...params);
  }
}

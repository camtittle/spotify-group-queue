import { Component, OnInit } from '@angular/core';
import { AuthenticationService, HubConnectionService } from '../../services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {

  public username: string;
  public registrationError = '';

  public tempMessageOutput = '';

  private usernameInput: string;

  constructor(private authenticationService: AuthenticationService,
              private router: Router,
              private hubConnectionService: HubConnectionService) { }

  ngOnInit() {

    // Test
    const conn = this.hubConnectionService.connection;
    // Subscribe to message event
    this.hubConnectionService.receieveMessage$.subscribe(message => {
      this.tempMessageOutput = `${message.user}: ${message.message}`;
    });

  }

  onRegisterClick() {
    this.authenticationService.register(this.usernameInput).subscribe(result => {
      this.router.navigate(['/find']);
    },
    err => this.registrationError = err);
  }

}

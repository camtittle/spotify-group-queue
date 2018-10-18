import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.css']
})
export class QueueComponent implements OnInit {

  public username: string;
  public registrationError = '';

  private usernameInput: string;

  constructor(private userService: UserService,
              private authenticationService: AuthenticationService) { }

  ngOnInit() {
  }

  onRegisterClick() {
    this.authenticationService.register(this.usernameInput).subscribe(result => {
      this.username = result.username;
      console.log(`Auth token: ${result.authToken}`);
    },
    err => this.registrationError = err);
  }

}

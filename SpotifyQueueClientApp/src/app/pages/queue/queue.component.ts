import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-queue',
  templateUrl: './queue.component.html',
  styleUrls: ['./queue.component.css']
})
export class QueueComponent implements OnInit {

  private result: any;
  private usernameInput: string;

  constructor(private userService: UserService,
              private authenticationService: AuthenticationService) { }

  ngOnInit() {
  }

  onLoginButtonClick(event: any) {
    this.authenticationService.register(this.usernameInput).subscribe(result => {

    });
  }

  onButtonClick(event: any) {
    this.userService.getMe().subscribe(x => {
      this.result = JSON.stringify(x);
    });
  }

}

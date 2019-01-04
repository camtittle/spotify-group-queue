import { Component, OnInit } from '@angular/core';
import { AuthenticationService, HubConnectionService } from '../../services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {

  public registrationError = '';

  public usernameInput: string;

  constructor(private authenticationService: AuthenticationService,
              private router: Router) { }

  ngOnInit() {
  }

  public onRegisterClick() {
    this.authenticationService.register(this.usernameInput).subscribe(async result => {
      await this.router.navigate(['/find']);
    },
    err => this.registrationError = err);
  }

}

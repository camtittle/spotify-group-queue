import { Component, OnInit } from '@angular/core';
import { UserService, AuthenticationService } from '../../services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {

  public username: string;
  public registrationError = '';

  private usernameInput: string;

  constructor(private authenticationService: AuthenticationService,
              private router: Router) { }

  ngOnInit() {
  }

  onRegisterClick() {
    this.authenticationService.register(this.usernameInput).subscribe(result => {
      this.router.navigate(['/find']);
    },
    err => this.registrationError = err);
  }

}

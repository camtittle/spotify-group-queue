import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {

  public loading = false;
  public registrationError = '';
  public usernameInput: string;

  constructor(private authenticationService: AuthenticationService,
              private router: Router) { }

  ngOnInit() {
  }

  public onRegisterClick() {
    this.loading = true;
    this.authenticationService.register(this.usernameInput).subscribe(async result => {
      if (result.currentParty) {
        await this.router.navigate(['party', 'queue']);
      } else {
        await this.router.navigate(['find']);
      }
    },
    err => {
      this.loading = false;
      this.registrationError = err;
    });
  }

}

import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticationService } from '../services';
import { AccessToken } from '../models';

/*
* Protects authorized routes by redirecting to welcome
*/
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  private currentUser: AccessToken;

  constructor(private router: Router,
              private authService: AuthenticationService) {
    authService.currentUser$.subscribe(accessToken => {
      this.currentUser = accessToken;
    });
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean> | boolean {

    if (this.currentUser) {
      return true;
    }

    this.router.navigate([''], { queryParams: { returnUrl: state.url }});
  }

  canActivateChild(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> | boolean {
    return this.canActivate(next, state);
  }
}

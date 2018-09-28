import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

/*
* Protects authorized routes by redirecting to welcome
*/
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) {
  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
      
    if (localStorage.getItem('currentUser')) {
      return true;
    }

    this.router.navigate(['/welcome'], { queryParams: { returnUrl: state.url }});
  }
}

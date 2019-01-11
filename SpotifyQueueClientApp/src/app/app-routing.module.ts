import { AuthGuard } from './guards/auth.guard';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IndexComponent, QueueComponent, FindComponent, CreateComponent, SearchComponent } from './pages';
import { PartyMembershipGuardGuard } from './guards/party-membership-guard.guard';
import { SpotifyCallbackComponent } from './pages/spotify-callback/spotify-callback.component';

const appRoutes: Routes = [
  { path: '', component: IndexComponent },
  { path: 'find', component: FindComponent },
  { path: 'create', component: CreateComponent, canActivate: [AuthGuard] },
  {
    path: 'search',
    component: SearchComponent,
    canActivate: [AuthGuard, PartyMembershipGuardGuard]
  },
  {
    path: 'queue',
    component: QueueComponent,
    canActivate: [AuthGuard, PartyMembershipGuardGuard]
  },
  {
    path: 'spotifycallback',
    component: SpotifyCallbackComponent
  }
];

@NgModule({
  imports: [ RouterModule.forRoot(appRoutes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }

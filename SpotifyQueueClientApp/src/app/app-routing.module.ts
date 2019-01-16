import { AuthGuard } from './guards/auth.guard';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IndexComponent, QueueComponent, FindComponent, CreateComponent, SearchComponent } from './pages';
import { PartyMembershipGuard } from './guards/party-membership-guard';
import { SpotifyCallbackComponent } from './pages/spotify-callback/spotify-callback.component';
import { PartyComponent } from './features/party/party.component';

const appRoutes: Routes = [
  {
    path: '',
    component: IndexComponent
  },
  {
    path: 'spotifycallback',
    component: SpotifyCallbackComponent
  },
  {
    path: 'find',
    component: FindComponent
  },
  {
    path: 'create',
    component: CreateComponent,
    canActivate: [AuthGuard] },
  {
    path: 'party',
    component: PartyComponent,
    canActivate: [AuthGuard, PartyMembershipGuard],
    canActivateChild: [AuthGuard, PartyMembershipGuard],
    children: [
      {
        path: 'queue',
        component: QueueComponent
      },
      {
        path: 'search',
        component: SearchComponent
      }
    ]
  }
];

@NgModule({
  imports: [ RouterModule.forRoot(appRoutes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }

import { AuthGuard } from './guards/auth.guard';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IndexComponent, QueueComponent, FindComponent, CreateComponent } from './pages';

const appRoutes: Routes = [
  { path: '', component: IndexComponent },
  { path: 'find', component: FindComponent },
  { path: 'create', component: CreateComponent, canActivate: [AuthGuard] },
  { path: 'queue', component: QueueComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [ RouterModule.forRoot(appRoutes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { QueueComponent } from './pages/queue/queue.component';

const appRoutes: Routes = [
  { path: 'queue', component: QueueComponent }
];

@NgModule({
  imports: [ RouterModule.forRoot(appRoutes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }

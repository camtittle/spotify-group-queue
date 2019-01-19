import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PartyComponent } from './party.component';
import { QueueComponent } from './queue/queue.component';
import { SearchComponent } from './search/search.component';
import { BsDropdownModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from '../../app-routing.module';
import { PipesModule } from '../../pipes/pipes.module';
import { NotificationComponent } from './notification/notification.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    AppRoutingModule,
    BsDropdownModule,
    PipesModule,
    SharedModule
  ],
  declarations: [
    PartyComponent,
    QueueComponent,
    SearchComponent,
    NotificationComponent
  ],
  exports: [
    PartyComponent
  ]
})
export class PartyModule { }

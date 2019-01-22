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
import { MembersComponent } from './members/members.component';
import { DevicesComponent } from './devices/devices.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    AppRoutingModule,
    BsDropdownModule,
    PipesModule,
    SharedModule,
    BrowserAnimationsModule
  ],
  declarations: [
    PartyComponent,
    QueueComponent,
    SearchComponent,
    NotificationComponent,
    MembersComponent,
    DevicesComponent
  ],
  exports: [
    PartyComponent
  ]
})
export class PartyModule { }

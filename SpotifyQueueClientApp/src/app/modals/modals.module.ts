import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PendingMemberRequestComponent } from './pending-member-request/pending-member-request.component';
import { ModalModule } from 'ngx-bootstrap';

@NgModule({
  imports: [
    CommonModule,
    ModalModule
  ],
  declarations: [
    PendingMemberRequestComponent
  ],
  entryComponents: [
    PendingMemberRequestComponent
  ],
  exports: [
  ]
})
export class ModalsModule { }

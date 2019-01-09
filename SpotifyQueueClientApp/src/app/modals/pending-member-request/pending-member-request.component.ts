import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';
import { PendingMemberRequest } from '../../models';
import { SignalRService } from '../../services';

@Component({
  selector: 'app-pending-member-request',
  templateUrl: './pending-member-request.component.html',
  styleUrls: ['./pending-member-request.component.css']
})
export class PendingMemberRequestComponent implements OnInit {

  public request: PendingMemberRequest;

  constructor(public modalRef: BsModalRef,
              private signalRService: SignalRService) { }

  ngOnInit() {
  }

  public async onActionClick(accept: boolean) {
    if (this.request) {
      await this.signalRService.invoke('acceptPendingMember', this.request.id, accept);
    }
    this.modalRef.hide();
  }

}

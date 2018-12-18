import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';
import { PendingMemberRequest } from '../../models/pending-member-request.model';

@Component({
  selector: 'app-pending-member-request',
  templateUrl: './pending-member-request.component.html',
  styleUrls: ['./pending-member-request.component.css']
})
export class PendingMemberRequestComponent implements OnInit {

  public request: PendingMemberRequest;

  constructor(public modalRef: BsModalRef) { }

  ngOnInit() {
  }

}

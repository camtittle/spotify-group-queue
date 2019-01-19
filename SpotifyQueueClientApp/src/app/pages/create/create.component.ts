import { Component, OnInit } from '@angular/core';
import { PartyService } from '../../services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

  public nameInput: string;
  public creationError = '';

  constructor(private partyService: PartyService,
              private router: Router) { }

  ngOnInit() {
  }

  public onCreateClick() {
    this.partyService.create(this.nameInput).subscribe(async party => {
      await this.router.navigate(['party', 'queue']);
    }, err => this.creationError = err);
  }

}

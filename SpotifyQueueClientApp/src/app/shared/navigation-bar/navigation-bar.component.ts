import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.scss']
})
export class NavigationBarComponent implements OnInit {

  @Input() back: string[];
  @Input() title: string;

  @Input() actionIconClass: string;
  @Output() action = new EventEmitter();

  constructor(private router: Router) { }

  ngOnInit() {
  }

  public async onClickBackButton() {
    if (this.back && this.back.length > 0) {
      await this.router.navigate(this.back);
    }
    return;
  }

  public onClickActionButton() {
    this.action.emit();
  }

}

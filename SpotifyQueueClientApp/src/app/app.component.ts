import { Component, OnChanges, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnChanges {
  title = 'app';

  ngOnInit() {
    document.getElementById('initial-loader').remove();
  }

  ngOnChanges() {
    document.getElementsByTagName('app-navigation-bar')
  }
}





import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationBarComponent } from './navigation-bar/navigation-bar.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    NavigationBarComponent
  ],
  exports: [
    NavigationBarComponent
  ]
})
export class SharedModule { }

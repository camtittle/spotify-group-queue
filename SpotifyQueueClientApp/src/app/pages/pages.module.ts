import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QueueComponent } from './queue/queue.component';

import { FormsModule } from '@angular/forms';
import { IndexComponent } from './index/index.component';
import { CreateComponent } from './create/create.component';
import { FindComponent } from './find/find.component';
import { SearchComponent } from './search/search.component';
import { PipesModule } from '../pipes/pipes.module';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    PipesModule
  ],
  declarations: [
    QueueComponent,
    IndexComponent,
    CreateComponent,
    FindComponent,
    SearchComponent
  ],
  exports: [
    QueueComponent,
    IndexComponent,
    CreateComponent
  ]
})
export class PagesModule { }

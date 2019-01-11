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
import { SpotifyCallbackComponent } from './spotify-callback/spotify-callback.component';
import { BsDropdownModule } from 'ngx-bootstrap';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    PipesModule,
    BsDropdownModule
  ],
  declarations: [
    QueueComponent,
    IndexComponent,
    CreateComponent,
    FindComponent,
    SearchComponent,
    SpotifyCallbackComponent
  ],
  exports: [
    QueueComponent,
    IndexComponent,
    CreateComponent
  ]
})
export class PagesModule { }

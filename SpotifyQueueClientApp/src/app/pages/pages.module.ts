import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QueueComponent } from '../features/party/queue/queue.component';

import { FormsModule } from '@angular/forms';
import { IndexComponent } from './index/index.component';
import { CreateComponent } from './create/create.component';
import { FindComponent } from './find/find.component';
import { SearchComponent } from '../features/party/search/search.component';
import { PipesModule } from '../pipes/pipes.module';
import { SpotifyCallbackComponent } from './spotify-callback/spotify-callback.component';
import { BsDropdownModule } from 'ngx-bootstrap';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    PipesModule,
    SharedModule
  ],
  declarations: [
    IndexComponent,
    CreateComponent,
    FindComponent,
    SpotifyCallbackComponent
  ],
  exports: [
    IndexComponent,
    CreateComponent
  ]
})
export class PagesModule { }

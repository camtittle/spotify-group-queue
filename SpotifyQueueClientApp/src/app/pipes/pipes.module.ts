import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JoinArtistNamesPipe } from './join-artist-names.pipe';
import { FormatMillisDurationPipe } from './format-millis-duration.pipe';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    JoinArtistNamesPipe,
    FormatMillisDurationPipe
  ],
  exports: [
    JoinArtistNamesPipe,
    FormatMillisDurationPipe
  ]
})
export class PipesModule { }

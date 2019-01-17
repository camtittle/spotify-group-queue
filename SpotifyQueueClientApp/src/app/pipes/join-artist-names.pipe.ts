import { Pipe, PipeTransform } from '@angular/core';
import { SpotifyArtist } from '../models/spotify/spotify-track.model';

@Pipe({
  name: 'joinArtistNames'
})
export class JoinArtistNamesPipe implements PipeTransform {

  transform(value: SpotifyArtist[]): string {
    return value.map(x => x.name).join(', ');
  }

}

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SpotifyService } from '../../services/spotify.service';

@Component({
  selector: 'app-spotify-callback',
  template: '',
  styles: ['']
})
export class SpotifyCallbackComponent implements OnInit {

  constructor(private route: ActivatedRoute,
              private router: Router,
              private spotifyService: SpotifyService) { }

  ngOnInit() {
    // Check for access token
    this.route.queryParams.subscribe(async params => {
      const code = params['code'];
      const state = params['state'];
      const error = params['error'];

      if (!code || !state) {
        if (error) {
          // TODO: do something with this
          console.warn('Spotify auth error: ' + error);
        }
      } else {
        await this.handleCode(state, code);
      }

      await this.navigateAway();
    });
  }

  private async handleCode(state: string, code: string) {
    if (this.spotifyService.validateState(state)) {
      await this.spotifyService.authorizeWithCode(code);
    }
  }

  private async navigateAway() {
    await this.router.navigate(['party', 'queue'], {queryParams: []});
  }

}

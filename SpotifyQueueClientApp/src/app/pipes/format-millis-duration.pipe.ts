import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatMillisDuration'
})
export class FormatMillisDurationPipe implements PipeTransform {

  transform(millis: number): string {
    const totalSecs = Math.floor(millis / 1000);
    const totalMins = Math.floor(totalSecs / 60);

    const secs = totalSecs % 60;
    const mins = totalMins % 60;
    const hrs = Math.floor(totalMins / 60);

    const formatSecs = this.format(secs);
    const formatMins = this.format(mins, hrs > 0);
    const formatHrs = hrs > 0 ? hrs + ':' : '';
    return formatHrs + formatMins + ':' + formatSecs;
  }

  private format(value: number, leadingZero = true): string {
    let result = '' + value;
    if (leadingZero && value < 10) {
      result = '0' + result;
    }
    return result;
  }

}

import { animate, animation, style, transition, trigger, useAnimation } from '@angular/animations';


const fadeInAnimation = animation([
  style({opacity: 0}),
  animate(500, style({opacity: 1}))
]);

const fadeOutAnimation = animation([
  animate(500, style({opacity: 0}))
]);

export const fadeInOut = trigger('fadeInOut', [
  transition(':enter', [   // :enter is alias to 'void => *'
    useAnimation(fadeInAnimation)
  ]),
  transition(':leave', [   // :leave is alias to '* => void'
    useAnimation(fadeOutAnimation)
  ])
]);

export const fadeIn = trigger('fadeIn', [
  transition(':enter', [   // :enter is alias to 'void => *'
    useAnimation(fadeInAnimation)
  ])
]);

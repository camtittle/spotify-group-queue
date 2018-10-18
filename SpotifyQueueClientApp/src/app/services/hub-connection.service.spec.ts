import { TestBed, inject } from '@angular/core/testing';

import { HubConnectionService } from './hub-connection.service';

describe('HubConnectionService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HubConnectionService]
    });
  });

  it('should be created', inject([HubConnectionService], (service: HubConnectionService) => {
    expect(service).toBeTruthy();
  }));
});

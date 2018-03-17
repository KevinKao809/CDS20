import { TestBed, inject } from '@angular/core/testing';

import { TransferTimeService } from './transfer-time.service';

describe('TransferTimeService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TransferTimeService]
    });
  });

  it('should be created', inject([TransferTimeService], (service: TransferTimeService) => {
    expect(service).toBeTruthy();
  }));
});

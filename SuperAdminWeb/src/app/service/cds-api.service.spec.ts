import { TestBed, inject } from '@angular/core/testing';

import { CdsApiService } from './cds-api.service';

describe('CdsApiService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CdsApiService]
    });
  });

  it('should be created', inject([CdsApiService], (service: CdsApiService) => {
    expect(service).toBeTruthy();
  }));
});

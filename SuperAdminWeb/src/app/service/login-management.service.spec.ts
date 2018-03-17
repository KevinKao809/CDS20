import { TestBed, inject } from '@angular/core/testing';

import { LoginManagementService } from './login-management.service';

describe('LoginManagementService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LoginManagementService]
    });
  });

  it('should be created', inject([LoginManagementService], (service: LoginManagementService) => {
    expect(service).toBeTruthy();
  }));
});

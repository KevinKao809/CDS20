import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceClassComponent } from './device-class.component';

describe('DeviceClassComponent', () => {
  let component: DeviceClassComponent;
  let fixture: ComponentFixture<DeviceClassComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeviceClassComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeviceClassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

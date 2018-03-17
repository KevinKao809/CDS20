import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditIothubMonitorComponent } from './edit-iothub-monitor.component';

describe('EditIothubMonitorComponent', () => {
  let component: EditIothubMonitorComponent;
  let fixture: ComponentFixture<EditIothubMonitorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditIothubMonitorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditIothubMonitorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

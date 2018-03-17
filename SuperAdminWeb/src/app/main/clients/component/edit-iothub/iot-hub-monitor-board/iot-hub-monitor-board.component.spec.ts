import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IotHubMonitorBoardComponent } from './iot-hub-monitor-board.component';

describe('IotHubMonitorBoardComponent', () => {
  let component: IotHubMonitorBoardComponent;
  let fixture: ComponentFixture<IotHubMonitorBoardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IotHubMonitorBoardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IotHubMonitorBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

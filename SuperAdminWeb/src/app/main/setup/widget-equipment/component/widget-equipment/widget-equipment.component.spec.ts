import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetEquipmentComponent } from './widget-equipment.component';

describe('WidgetEquipmentComponent', () => {
  let component: WidgetEquipmentComponent;
  let fixture: ComponentFixture<WidgetEquipmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetEquipmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetEquipmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

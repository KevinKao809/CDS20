import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetCompanyComponent } from './widget-company.component';

describe('WidgetCompanyComponent', () => {
  let component: WidgetCompanyComponent;
  let fixture: ComponentFixture<WidgetCompanyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetCompanyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetCompanyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

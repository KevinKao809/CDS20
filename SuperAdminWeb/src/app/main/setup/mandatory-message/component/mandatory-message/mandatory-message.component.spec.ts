import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MandatoryMessageComponent } from './mandatory-message.component';

describe('MandatoryMessageComponent', () => {
  let component: MandatoryMessageComponent;
  let fixture: ComponentFixture<MandatoryMessageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MandatoryMessageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MandatoryMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

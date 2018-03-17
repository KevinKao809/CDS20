import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditIothubDetailComponent } from './edit-iothub-detail.component';

describe('EditIothubDetailComponent', () => {
  let component: EditIothubDetailComponent;
  let fixture: ComponentFixture<EditIothubDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditIothubDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditIothubDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

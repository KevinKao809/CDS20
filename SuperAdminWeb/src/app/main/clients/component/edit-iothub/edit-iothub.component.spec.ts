import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditIothubComponent } from './edit-iothub.component';

describe('EditIothubComponent', () => {
  let component: EditIothubComponent;
  let fixture: ComponentFixture<EditIothubComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditIothubComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditIothubComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

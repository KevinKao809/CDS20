import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../material/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';


import { EnrollmentRoutingModule } from './enrollment-routing.module';
import { EnrollmentComponent } from './component/enrollment/enrollment.component';

@NgModule({
  imports: [
    CommonModule,
    EnrollmentRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule
  ],
  declarations: [EnrollmentComponent]
})
export class EnrollmentModule { }

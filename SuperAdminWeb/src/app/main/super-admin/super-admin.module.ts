import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SuperAdminRoutingModule } from './super-admin-routing.module';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../material/material.module';
import { SharedModule } from '../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';
import { SuperAdminComponent } from './component/super-admin/super-admin.component';

@NgModule({
  imports: [
    CommonModule,
    SuperAdminRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule
  ],
  declarations: [SuperAdminComponent]
})
export class SuperAdminModule { }

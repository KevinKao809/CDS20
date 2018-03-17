import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../material/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';

import { PermissionRoutingModule } from './permission-routing.module';
import { PermissionComponent } from './component/permission/permission.component';

@NgModule({
  imports: [
    CommonModule,
    PermissionRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule
  ],
  declarations: [PermissionComponent]
})
export class PermissionModule { }

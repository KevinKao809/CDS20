import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../material/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';

import { DeviceClassRoutingModule } from './device-class-routing.module';
import { DeviceClassComponent } from './component/device-class/device-class.component';

@NgModule({
  imports: [
    CommonModule,
    DeviceClassRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule
  ],
  declarations: [DeviceClassComponent]
})
export class DeviceClassModule { }

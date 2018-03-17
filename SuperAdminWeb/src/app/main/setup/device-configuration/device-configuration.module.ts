import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../material/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';

import { DeviceConfigurationRoutingModule } from './device-configuration-routing.module';
import { DeviceConfigurationComponent } from './component/device-configuration/device-configuration.component';

@NgModule({
  imports: [
    CommonModule,
    DeviceConfigurationRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule
  ],
  declarations: [DeviceConfigurationComponent]
})
export class DeviceConfigurationModule { }

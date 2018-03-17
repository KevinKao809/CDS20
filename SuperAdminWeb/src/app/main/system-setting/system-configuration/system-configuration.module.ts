import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../material/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';
import { SystemConfigurationRoutingModule } from './system-configuration-routing.module';
import { SystemConfigurationComponent } from './component/system-configuration/system-configuration.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule,
    SystemConfigurationRoutingModule
  ],
  declarations: [SystemConfigurationComponent]
})
export class SystemConfigurationModule { }

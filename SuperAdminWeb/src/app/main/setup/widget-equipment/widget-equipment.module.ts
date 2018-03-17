import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../material/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';
import { WidgetEquipmentRoutingModule } from './widget-equipment-routing.module';
import { WidgetEquipmentComponent } from './component/widget-equipment/widget-equipment.component';

@NgModule({
  imports: [
    CommonModule,
    WidgetEquipmentRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule
  ],
  declarations: [WidgetEquipmentComponent]
})
export class WidgetEquipmentModule { }

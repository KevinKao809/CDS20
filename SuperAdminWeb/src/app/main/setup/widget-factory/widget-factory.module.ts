import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../material/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';
import { WidgetFactoryRoutingModule } from './widget-factory-routing.module';
import { WidgetFactoryComponent } from './component/widget-factory/widget-factory.component';

@NgModule({
  imports: [
    CommonModule,
    WidgetFactoryRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule
  ],
  declarations: [WidgetFactoryComponent]
})
export class WidgetFactoryModule { }

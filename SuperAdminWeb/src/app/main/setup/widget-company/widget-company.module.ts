import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../material/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';
import { WidgetCompanyRoutingModule } from './widget-company-routing.module';
import { WidgetCompanyComponent } from './component/widget-company/widget-company.component';

@NgModule({
  imports: [
    CommonModule,
    WidgetCompanyRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule
  ],
  declarations: [WidgetCompanyComponent]
})
export class WidgetCompanyModule { }

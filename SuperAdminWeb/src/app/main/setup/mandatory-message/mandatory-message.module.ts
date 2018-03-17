import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../../material/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';

import { MandatoryMessageRoutingModule } from './mandatory-message-routing.module';
import { MandatoryMessageComponent } from './component/mandatory-message/mandatory-message.component';

@NgModule({
  imports: [
    CommonModule,
    MandatoryMessageRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule
  ],
  declarations: [MandatoryMessageComponent]
})
export class MandatoryMessageModule { }

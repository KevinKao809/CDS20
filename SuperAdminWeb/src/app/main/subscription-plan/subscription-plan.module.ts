import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../material/material.module';
import { SharedModule } from '../shared/shared.module';
import { CustomFormsModule } from 'ng2-validation';
import { SubscriptionPlanRoutingModule } from './subscription-plan-routing.module';
import { SubscriptionPlanComponent } from './component/subscription-plan/subscription-plan.component';

@NgModule({
  imports: [
    CommonModule,
    SubscriptionPlanRoutingModule,
    FormsModule,
    MaterialModule,
    SharedModule,
    CustomFormsModule

  ],
  declarations: [SubscriptionPlanComponent]
})
export class SubscriptionPlanModule { }

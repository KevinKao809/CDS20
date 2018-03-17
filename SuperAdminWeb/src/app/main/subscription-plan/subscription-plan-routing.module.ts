import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SubscriptionPlanComponent } from './component/subscription-plan/subscription-plan.component';

const routes: Routes = [
  {
    path:'',
    component: SubscriptionPlanComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SubscriptionPlanRoutingModule { }

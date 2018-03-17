import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {WidgetEquipmentComponent} from './component/widget-equipment/widget-equipment.component';
const routes: Routes = [ {
  path:'',
  component: WidgetEquipmentComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WidgetEquipmentRoutingModule { }

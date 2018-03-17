import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {WidgetFactoryComponent} from './component/widget-factory/widget-factory.component';
const routes: Routes = [
  {
    path:'',
    component: WidgetFactoryComponent
  }
  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WidgetFactoryRoutingModule { }

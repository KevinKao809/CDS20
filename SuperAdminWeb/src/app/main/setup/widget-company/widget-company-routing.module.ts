import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {WidgetCompanyComponent} from './component/widget-company/widget-company.component';
import { patch } from 'webdriver-js-extender';
const routes: Routes = [
  
  {
    path: '',
    component:  WidgetCompanyComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WidgetCompanyRoutingModule { }

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DeviceClassComponent } from './component/device-class/device-class.component';

const routes: Routes = [
  {
    path: '',
    component: DeviceClassComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DeviceClassRoutingModule { }

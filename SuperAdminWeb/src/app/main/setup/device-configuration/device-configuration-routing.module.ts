import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DeviceConfigurationComponent } from './component/device-configuration/device-configuration.component';

const routes: Routes = [
  {
    path: '',
    component: DeviceConfigurationComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DeviceConfigurationRoutingModule { }

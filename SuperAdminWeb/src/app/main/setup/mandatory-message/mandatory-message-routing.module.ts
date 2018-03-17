import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MandatoryMessageComponent } from './component/mandatory-message/mandatory-message.component';

const routes: Routes = [
  {
    path: '',
    component: MandatoryMessageComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MandatoryMessageRoutingModule { }

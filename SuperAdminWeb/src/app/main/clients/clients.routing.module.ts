import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ClientsComponent } from './component/clinets/clients.component';

const clientsRoutes: Routes = [
  {
    path: '',
      component: ClientsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(clientsRoutes)],
  exports: [
    RouterModule
  ]
})
export class ClientsRoutingModule { }

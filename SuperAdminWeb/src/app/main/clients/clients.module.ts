import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClientsRoutingModule } from './clients.routing.module';
import { SharedModule } from '../shared/shared.module';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../material/material.module';
import { ClipboardModule } from 'ngx-clipboard';
import { CustomFormsModule } from 'ng2-validation';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { NgxGaugeModule } from 'ngx-gauge';

import { ClientsComponent } from './component/clinets/clients.component';
import { EditClientsComponent } from './component/edit-clients/edit-clients.component';
import { EditEmployeeComponent } from './component/edit-employee/edit-employee.component';
import { EditIothubComponent } from './component/edit-iothub/edit-iothub.component';
import { EditIothubDetailComponent } from './component/edit-iothub/edit-iothub-detail/edit-iothub-detail.component';
import { EditIothubMonitorComponent  } from './component/edit-iothub/edit-iothub-monitor/edit-iothub-monitor.component';
import { IotHubMonitorBoardComponent } from './component/edit-iothub/iot-hub-monitor-board/iot-hub-monitor-board.component';
import { EditDashboardComponent } from './component/edit-dashboard/edit-dashboard.component';
import { EditSubscriptionComponent } from './component/edit-subscription/edit-subscription.component';



@NgModule({
  imports: [
    CommonModule,
    ClientsRoutingModule,
    SharedModule,
    FormsModule,
    MaterialModule,
    ClipboardModule,
    CustomFormsModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    NgxGaugeModule
  ],
  declarations: [
    ClientsComponent,
    EditClientsComponent,
    EditEmployeeComponent,
    EditIothubComponent,
    EditIothubDetailComponent,
    EditIothubMonitorComponent,
    IotHubMonitorBoardComponent,
    EditDashboardComponent,
    EditSubscriptionComponent
  ],
  entryComponents: [
    IotHubMonitorBoardComponent
  ]
})
export class ClientsModule { }

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuardService } from '../service/auth.guard.service';
import { MainComponent } from './main.component';


const mainRoutes: Routes = [
  {
    path: '',
      canActivate: [AuthGuardService],
      component: MainComponent,
      children: [
        {
          path: 'clients',
          loadChildren: 'app/main/clients/clients.module#ClientsModule',
          data: { preload: true }
        },
        {
          path: 'enrollment',
          loadChildren: 'app/main/equipment/enrollment/enrollment.module#EnrollmentModule',
          data: { preload: true }
        },
        {
          path: 'superAdmin',
          loadChildren: 'app/main/super-admin/super-admin.module#SuperAdminModule',
          data: { preload: true }
        },
        {
          path: 'permission',
          loadChildren: 'app/main/setup/permission/permission.module#PermissionModule',
          data: { preload: true }
        },
        {
          path: 'deviceClass',
          loadChildren: 'app/main/setup/device-class/device-class.module#DeviceClassModule',
          data: { preload: true }
        },
        {
          path: 'device-configuration',
          loadChildren: 'app/main/setup/device-configuration/device-configuration.module#DeviceConfigurationModule',
          data: { preload: true }
        },
        {
          path: 'mandatory-message',
          loadChildren: 'app/main/setup/mandatory-message/mandatory-message.module#MandatoryMessageModule',
          data: { preload: true }
        },
        {
          path: 'subscriptionPlan',
          loadChildren: 'app/main/subscription-plan/subscription-plan.module#SubscriptionPlanModule',
          data: { preload: true }
        },
        {
          path: 'widgetCompany',
          loadChildren: 'app/main/setup/widget-company/widget-company.module#WidgetCompanyModule',
          data: { preload: true }
        },
        {
          path: 'widgetFactory',
          loadChildren: 'app/main/setup/widget-factory/widget-factory.module#WidgetFactoryModule',
          data: { preload: true }
        },
        {
          path: 'widgetEquipment',
          loadChildren: 'app/main/setup/widget-equipment/widget-equipment.module#WidgetEquipmentModule',
          data: { preload: true }
        },
        {
          path: 'systemConfiguration',
          loadChildren: 'app/main/system-setting/system-configuration/system-configuration.module#SystemConfigurationModule',
          data: { preload: true }
        },
        {
          path: 'redisCache',
          loadChildren: 'app/main/system-setting/redis-cache/redis-cache.module#RedisCacheModule',
          data: { preload: true }
        },
        {
          path: '',
          redirectTo: '/main/clients',
          pathMatch: 'full'
        }
      ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(mainRoutes)],
  exports: [
    RouterModule
  ],
  providers: [AuthGuardService],
})
export class MainRoutingModule { }

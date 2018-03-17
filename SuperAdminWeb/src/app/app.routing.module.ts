import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { AppErrorComponent } from './error-page/app.error.component';

const appRoutes: Routes = [
  {
    path: 'login',
    loadChildren: 'app/login/login.module#LoginModule',
    data: { preload: true }
  },
  {
    path: 'main',
    loadChildren: 'app/main/main.module#MainModule',
    data: { preload: true }
  },
  {
    path: '',
    redirectTo: '/main/clients',
    pathMatch: 'full'
  },
  {
    path: 'errorpage/:id',
    component: AppErrorComponent
  },
  {
    path: '**',
    redirectTo: '/errorpage/404',
    pathMatch: 'full'
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes, {useHash: true, enableTracing: false})
  ],
  exports: [
    RouterModule
  ],
})
export class AppRoutingModule { }

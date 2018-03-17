import { NgModule } from '@angular/core';
import { RedisCacheComponent } from './component/redis-cache/redis-cache.component';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    component: RedisCacheComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RedisCacheRoutingModule { }

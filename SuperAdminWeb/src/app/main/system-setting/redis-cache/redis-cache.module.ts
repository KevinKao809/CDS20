import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RedisCacheRoutingModule } from './redis-cache-routing.module';
import { RedisCacheComponent } from './component/redis-cache/redis-cache.component';

@NgModule({
  imports: [
    CommonModule,
    RedisCacheRoutingModule
  ],
  declarations: [RedisCacheComponent]
})
export class RedisCacheModule { }

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MainRoutingModule } from './main.routing.module';
import { MaterialModule } from '../material/material.module';
import { MainComponent } from './main.component';
import { SharedModule } from './shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    MainRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    SharedModule
  ],
  declarations: [
    MainComponent,
  ],
  providers: [],
})
export class MainModule { }

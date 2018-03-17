import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MaterialModule } from './material/material.module';

// routing
import { AppRoutingModule } from './app.routing.module';

// Component
import { AppComponent } from './app.component';
import { AppErrorComponent } from './error-page/app.error.component';

// Service
import { CdsApiService } from './service/cds-api.service';
import { LoginManagementService } from './service/login-management.service';
import { DataShareService } from './service/data-share.service';


@NgModule({
  declarations: [
    AppComponent,
    AppErrorComponent
  ],
  imports: [
    HttpClientModule,
    AppRoutingModule,
    NgbModule.forRoot(),
    BrowserModule,
    BrowserAnimationsModule,
    MaterialModule,
  ],
  providers: [CdsApiService, LoginManagementService, DataShareService],
  bootstrap: [AppComponent],
})
export class AppModule { }

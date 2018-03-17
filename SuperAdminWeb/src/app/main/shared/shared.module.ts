import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../material/material.module';
import { ExpandableTableComponent } from './expandable-table/expandable-table.component';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { ImageCropperModule } from 'ngx-image-cropper';
import { HtmlSanitizerPipe } from '../../pipe/html-sanitizer.pipe';
import { ClickStopPropagationDirective } from '../../directive/stop-propagation.directive';
import { PopupWindowComponent } from './popup-window.component/popup-window.component';
import { RemoveSpacePipe } from '../../pipe/remove-space.pipe';
import { CropImageComponent } from './crop-image/crop-image.component';



@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    MaterialModule,
    NgxDatatableModule,
    ImageCropperModule
  ],
  declarations: [ ExpandableTableComponent,
                  PopupWindowComponent,
                  HtmlSanitizerPipe,
                  RemoveSpacePipe,
                  ClickStopPropagationDirective,
                  CropImageComponent],
  exports: [
    ExpandableTableComponent,
    PopupWindowComponent,
    ClickStopPropagationDirective,
    CropImageComponent
  ]
})
export class SharedModule { }


import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { trigger, state, style, transition, animate, keyframes } from '@angular/animations';
import { DataShareService } from '../../../service/data-share.service';

@Component({
  selector: 'app-crop-image',
  templateUrl: './crop-image.component.html',
  styleUrls: ['./crop-image.component.scss'],
  animations: [
    trigger('fadeInOut', [
      state('fadeOut', style({opacity: '0'})),
      state('fadeIn', style({opacity: '1'})),
      transition('fadeOut <=> fadeIn', animate('200ms ease-in')),
    ]),
    trigger('slideIn', [
      state('out', style({'transform': 'translateY(-100%)'})),
      state('in', style({'transform': 'translateY(15%)'})),
      transition('out <=> in', animate('200ms ease-in')),
    ])
  ],
})
export class CropImageComponent implements OnInit {

  @Input() inputImageEvent: any;
  @Output() cropConfirm = new EventEmitter();

  isOpen = 'close';

  imageChangedEvent: any = '';
  croppedImage: any = '';
  fileData = {
    'url' : null,
    'file': null
  };


  constructor( private dataShareService: DataShareService) { }

  fileChangeEvent(event: any): void {

  }
  imageCropped(image: string) {
      this.croppedImage = image;
  }
  imageLoaded() {

  }
  loadImageFailed() {

  }

  closeCropDialog() {
    this.isOpen = 'close';
    setTimeout( () => this.cropConfirm.emit(true), 200);
  }

  confirmCrop() {
    this.fileData.url = this.croppedImage;
    this.fileData.file = this.dataURLtoFile();
    this.dataShareService.publishCropImageResult(this.fileData);
    this.closeCropDialog();
  }

  cancelCrop() {
    this.closeCropDialog();
  }

  dataURLtoFile() {

    const arr = this.croppedImage.split(','),
          mime =  arr[0].match(/:(.*?);/)[1],
          bstr = atob(arr[1]);

    let n = bstr.length;
    const u8arr = new Uint8Array(n);
    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }
    return new File([u8arr], 'image.png', {type: mime});
  }

  ngOnInit() {
    setTimeout( () => this.isOpen = 'open');

  }

}

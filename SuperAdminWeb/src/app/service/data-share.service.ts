import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Subscription } from 'rxjs/subscription';

@Injectable()
export class DataShareService {

  private popupMsg = new BehaviorSubject<any>(null);
  popupMsg$ = this.popupMsg.asObservable();

  private signalrMsg = new BehaviorSubject<any>(null);
  signalrMsg$ = this.signalrMsg.asObservable();

  private cropImageModalMsg = new BehaviorSubject<any>(null);
  cropImageModalMsg$ = this.cropImageModalMsg.asObservable();

  private cropImageModalResultMsg = new BehaviorSubject<any>(null);
  cropImageModalResultMsg$ = this.cropImageModalResultMsg.asObservable();

  constructor() {}

  /**
   * publishPopupWindowMsg: Trigger popup message showing
   * @param data : popupMsg Object
   */
  publishPopupWindowMsg(data) {
    this.popupMsg.next(data);
  }

  /**
   * publishSignalRMsg: publish signalr message
   * @param msg : received signalr push message
   */
  publishSignalRMsg(msg) {
    this.signalrMsg.next(msg);
  }


  publishCropImage(msg) {
    this.cropImageModalMsg.next(msg);
  }

  publishCropImageResult(msg) {
    this.cropImageModalResultMsg.next(msg);
  }

}

import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/subscription';
import 'rxjs/add/operator/skip';
import { DataShareService } from '../service/data-share.service';
import { LoginManagementService } from '../service/login-management.service';
import { trigger, state, style, transition, animate, keyframes } from '@angular/animations';

@Component({
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
  providers: [LoginManagementService],
  animations: [
    trigger('itemDetail', [
      state('collapsed', style({height: '0px', minHeight: '0', visibility: 'hidden'})),
      state('expanded', style({height: '*', visibility: 'visible'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
    trigger('sideBarCollapse', [
      state('show', style({width: '180px'})),
      state('hide', style({width: '60px'})),
      transition('show <=> hide', animate('160ms ease-in')),
    ]),
    trigger('sideBarElementShowOrHide', [
      state('show', style({opacity: '1'})),
      state('hide', style({opacity: '0'})),
      transition('hide <=> show', animate('100ms ease-in')),
    ]),
    trigger('mainContentWidth', [
      state('collapsed', style({width: 'calc(100% - 180px)'})),
      state('extended', style({width: 'calc(100% - 60px)'})),
      transition('extended <=> collapsed', animate('160ms ease-in')),
    ]),
  ],
})
export class MainComponent implements OnInit, OnDestroy {

  popWindowDisplayObj: any = {};

  headerInfo: any = {
    'userName': null,
    'userMail': null,
  };

  imageChangeEvent = null;

  private popupWindowSubscription: Subscription;
  private cropImageModalSubscription: Subscription;

  popUpCropImageDialog: Boolean = false;

  constructor(private loginManagementService: LoginManagementService, private sharedService: DataShareService) { }

  navBarExpandItem: Object = {
    'Equipment': false,
    'Setup': false,
    'Monitor': false,
    'SystemSetting': false
  };

  sideBarExpand: String = 'show';
  sideBarElementDisplay: String = 'show';
  mainContentWidth: String = 'collapsed';

  updateHeaderInfo() {
    const userInfo = JSON.parse(localStorage.getItem('cds-userInfo'));
    this.headerInfo.userName = userInfo.FirstName + ' ' + userInfo.LastName;
    this.headerInfo.userMail = userInfo.Email;
  }

  expandNavItemOrNot(item: string) {
    if (this.sideBarExpand === 'hide') {
      this.sideBarExpand = 'show';
      this.navBarExpandItem[item] = true;
      this.mainContentWidth = 'collapsed';
    }else {
      this.navBarExpandItem[item] = !this.navBarExpandItem[item];
    }
  }

  /**
   * nav bar collapse or expand
   */
  navBarCollpaseSwitch() {

    if (this.sideBarExpand === 'show') {
      this.navBarExpandItem['Equipment'] = false;
      this.navBarExpandItem['Setup'] = false;
      this.navBarExpandItem['Monitor'] = false;
      this.navBarExpandItem['SystemSetting'] = false;
      this.sideBarExpand = 'hide';
      this.mainContentWidth = 'extended';
      this.sideBarElementDisplay = 'hide';
    }else {
      this.sideBarExpand = 'show';
      this.mainContentWidth = 'collapsed';
      this.sideBarElementDisplay = 'show';
    }
  }

  cropFinish(cropFinish) {
    this.popUpCropImageDialog = false;
  }

  ngOnInit() {
    this.updateHeaderInfo();
    this.popupWindowSubscription = this.sharedService.popupMsg$.skip(1).subscribe(popupObj => {
      this.popWindowDisplayObj = popupObj;
    });

    this.cropImageModalSubscription = this.sharedService.cropImageModalMsg$.skip(1).subscribe(cropImageMsg => {
      this.imageChangeEvent = cropImageMsg.imageChangeEvent;
      this.popUpCropImageDialog = true;
    });

  }

  ngOnDestroy() {
    this.popupWindowSubscription.unsubscribe();
    this.cropImageModalSubscription.unsubscribe();
  }

  logout() {
    this.loginManagementService.logOut();
  }
}

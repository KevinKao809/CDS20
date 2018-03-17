import { Component, OnInit, OnChanges, Input } from '@angular/core';
import { trigger, state, style, transition, animate, keyframes } from '@angular/animations';

class SuccessMessage {
  text = '';
  icon = 'icon-icon-confirm';
  class = 'success';
}

class FailedMessage {
  text = '';
  icon = 'icon-icon-confirmation';
  class = 'error';
  errorCode = '';
  errorText = '';
}

class ConfirmDialog {
  text = '';
  icon = 'icon-icon-confirmation';
  posX = 0;
  posY = 0;
  confirm = () => null;
  cancel = () => null;
}

@Component({
  selector: 'app-popup-window',
  templateUrl: './popup-window.component.html',
  styleUrls: ['./popup-window.component.scss'],
  animations: [
    trigger('showMsg', [
      state('true', style({display: 'block', opacity: .8})),
      state('false', style({display: 'none', opacity: 0})),
      transition('0 <=> 1', animate('250ms linear')),
    ]),
  ],
})

export class PopupWindowComponent implements OnInit, OnChanges {

  public innerWidth: any;
  @Input() popUpWindowObj: any = {};
  @Input() popupTemplateContent: any;

  feedbackMessageInfo = {
    'text': '',
    'icon': '',
    'class': ''
  };

  errorFeedbackMessageInfo = {
    'text': '',
    'icon': '',
    'class': '',
    'errorCode': '',
    'errorText': ''
  };

  confirmMessageInfo = {
    'text': '',
    'icon': '',
    'posX': 0,
    'posY': 0,
    'confirm': () => null,
    'cancel': () => null
  };


  feedbackMessage = {
    'createClientSuccess': { text: 'Add Client Success' },
    'editClientSuccess': { text: 'Edit Client Success' },
    'deleteClientSuccess': { text: 'Delete Client Success' },
    'createEmployeeSuccess': { text: 'Add Employee Success' },
    'editEmployeeSuccess': { text: 'Edit Employee Success' },
    'deleteEmployeeSuccess': { text: 'Delete Employee Success' },
    'createIotHubSuccess': { text: 'Add IOT Hub Success'},
    'editIotHubSuccess': { text: 'Edit IOT Hub Success'},
    'deleteteIotHubSuccess': { text: 'Delete IOT Hub Success'},
    'createDashboardSuccess': { text: 'Add Dashboard Success'},
    'editDashboardSuccess': { text: 'Edit Dashboard Success'},
    'deleteDashboardSuccess': {text: 'Delete Dashboard Success'},
    'createClientsSubscriptionSuccess': { text: 'Add Client Subscription Plan Success'},
    'editClientsSubscriptionSuccess': { text: 'Edit Client Subscription Plan Success'},
    'cancelClientsSubscriptionSuccess': {text: 'Unsubscribe Client Subscription Plan Success'},
    'createDeviceClassSuccess': {text: 'Add Device Class Success' },
    'editDeviceClassSuccess': {text: 'Edit Device Class Success'},
    'deleteDeviceClassSuccess': {text: 'Delete Device Class Success'},
    'createSuperAdminSuccess': { text: 'Add SuperAdmin Success'},
    'editSuperAdminSuccess': { text: 'Edit SuperAdmin Success'},
    'deleteSuperAdminSuccess': { text: 'Delete SuperAdmin Success'},
    'createPermissionSuccess': { text: 'Add Permission Success'},
    'editPermissionSuccess': { text: 'Edit Permission Success'},
    'deletePermissionSuccess': { text: 'Delete Permission Success'},
    'createDeviceConfigurationSuccess': { text: 'Add Device Configuration Success'},
    'editDeviceConfigurationSuccess': { text: 'Edit Device Configuration Success'},
    'deleteDeviceConfigurationSuccess': { text: 'Delete Device Configuration Success'},
    'createMandatoryMessageSuccess': { text: 'Add MandatoryMessage Success'},
    'editMandatoryMessageSuccess': { text: 'Edit MandatoryMessage Success'},
    'deleteMandatoryMessageSuccess': { text: 'Delete MandatoryMessage Success'},
    'createSubscriptionPlanSuccess': { text: 'Add SubscriptionPlan Success'},
    'editSubscriptionPlanSuccess': { text: 'Edit SubscriptionPlan Success'},
    'deleteSubscriptionPlanSuccess': { text: 'Delete SubscriptionPlan Success'},
    'createWidgetCompanySuccess': { text: 'Add Company Widget Success'},
    'editWidgetCompanySuccess': { text: 'Edit Company Widget Success'},
    'deleteWidgetCompanySuccess': { text: 'Delete Company Widget Success'},
    'createWidgetFactorySuccess': { text: 'Add Factory Widget Success'},
    'editWidgetFactorySuccess': { text: 'Edit Factory Widget Success'},
    'deleteWidgetFactorySuccess': { text: 'Delete Factory Widget Success'},
    'createWidgetEquipmentSuccess': { text: 'Add Equipment Widget Success'},
    'editWidgetEquipmentSuccess': { text: 'Edit Equipment Widget Success'},
    'deleteWidgetEquipmentSuccess': { text: 'Delete Equipment Widget Success'},
    'editSystemConfigurationSuccess': {text: 'Edit System Configuration Success'}
  };

  confirmMsg = {
    'deleteClient': { text: 'Are you sure you want to delete this client?' },
    'deleteEmployee': { text: 'Are you sure you want to delete this employee?'},
    'deleteIotHub': { text: 'Are you sure you want to delete this Iot Hub?'},
    'deleteDashboard': { text: 'Are you sure you want to delete this dashboard?'},
    'cancelClientsSubscription': { text: 'Are you sure you want to unsubscribe this subscription plan?'},
    'deleteDeviceClass': { text: 'Are you sure you want to delete this Device Class?'},
    'deleteSuperAdmin': { text: 'Are you sure you want to delete this superAdmin?'},
    'deletePermission': { text: 'Are you sure you want to delete this Permission?'},
    'deleteDeviceConfiguration': { text: 'Are you sure you want to delete this Device Configuration?'},
    'deleteMandatoryMessage': { text: 'Are you sure you want to delete this Mandatory Message Element Definition?'},
    'deleteSubscriptionPlan': { text: 'Are you sure you want to delete this SubscriptionPlan?'},
    'deleteWidgetCompany': { text: 'Are you sure you want to delete this Company Widget?'},
    'deleteWidgetFactory': { text: 'Are you sure you want to delete this Factory Widget?'},
    'deleteWidgetEquipment': { text: 'Are you sure you want to delete this Equipment Widget?'},
  };

  errorMsg = {
    'errorMessage': {text: 'Error Occurred'}
  };

  showFeedbackMessageFlag: Boolean = false;
  showConfirmDialogFlag: Boolean = false;
  showErrorFeedbackMessageFlag: Boolean = false;

  constructor() { }

  displayFeedbackMessage() {
    this.showFeedbackMessageFlag = true;
    setTimeout(() => this.showFeedbackMessageFlag = false, '5000');
  }


  feedbackMessageHandler(feedbackId) {
    const msg = new SuccessMessage;
    msg.text = this.feedbackMessage[feedbackId].text;
    this.feedbackMessageInfo = msg;
    this.displayFeedbackMessage();
  }

  displayErrorFeedbackMessage() {
    this.showErrorFeedbackMessageFlag = true;
    setTimeout(() => this.showErrorFeedbackMessageFlag = false, '5000');
  }

  apiErrorMessageHandler(errorObject) {
    const msg = new FailedMessage;
    msg.text = this.errorMsg[errorObject.id].text;
    msg.errorCode = errorObject.errorCode;
    msg.errorText = errorObject.statusText;
    this.errorFeedbackMessageInfo = msg;
    this.displayErrorFeedbackMessage();
  }


  cancelConformDialog() {
    this.confirmMessageInfo = new ConfirmDialog();
    this.showConfirmDialogFlag = false;
  }


  confirmDialogHandler(confirmObj) {

    const confimDialog = new ConfirmDialog();

    confimDialog.text = this.confirmMsg[confirmObj.id].text;
    confimDialog.posX = confirmObj.position.x;
    confimDialog.posY = confirmObj.position.y;
    confimDialog.confirm = () => {
      confirmObj.confirmCallback();
      this.cancelConformDialog();
    };
    confimDialog.cancel = () => this.cancelConformDialog();
    this.innerWidth = window.innerWidth;
    this.confirmMessageInfo = confimDialog;
    this.showConfirmDialogFlag = true;
  }

  ngOnChanges() {

    if (this.popUpWindowObj && this.popUpWindowObj.type) {
      switch (this.popUpWindowObj.type) {

        case 'feedback':
          this.feedbackMessageHandler(this.popUpWindowObj.id);
        break;

        case 'confirm':
        this.confirmDialogHandler(this.popUpWindowObj);
        break;

        case 'APIError':
        this.apiErrorMessageHandler(this.popUpWindowObj);

      }
    }
  }

  ngOnInit() {
  }

}


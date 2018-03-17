import { Component, OnInit, Input, EventEmitter } from '@angular/core';
import { CdsApiService } from '../../../../service/cds-api.service';
import { DataShareService } from '../../../../service/data-share.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';

class EditInputInfo {
  name: String;
  description: String;
  defaultPlanDays: Number;
  defaultMaxMessageQuotaPerDay: String;
  defaultRatePer1KMessageIngestion: Number;
  defaultRatePer1KMessageHotStore: Number;
  defaultRatePer1KMessageColdStore: Number;
  defaultStoreHotMessage: Boolean;
  defaultStoreColdMessage: Boolean;
  defaultCosmosDBConnectionString: String;
  defaultCollectionTTL: Number;
  defaultCollectionReservedUnits: Number;
  defaultIoTHubConnectionString: String;
  defaultStorageConnectionString: String;

  constructor(name: String = null, description: String = null, defaultPlanDays: Number = null, defaultMaxMessageQuotaPerDay: String = null,
    defaultRatePer1KMessageIngestion: Number = null, defaultRatePer1KMessageHotStore: Number = null,
    defaultRatePer1KMessageColdStore: Number = null, defaultStoreHotMessage: Boolean = false, defaultStoreColdMessage: Boolean = false,
    defaultCosmosDBConnectionString: String = 'AccountEndpoint=', defaultCollectionTTL: Number = 86400,
    defaultCollectionReservedUnits: Number = null, defaultIoTHubConnectionString: String = 'HostName=',
    defaultStorageConnectionString: String = 'DefaultEndpointsProtocol=') {
    this.name = name;
    this.description = description;
    this.defaultPlanDays = defaultPlanDays;
    this.defaultMaxMessageQuotaPerDay = defaultMaxMessageQuotaPerDay;
    this.defaultRatePer1KMessageIngestion = defaultRatePer1KMessageIngestion;
    this.defaultRatePer1KMessageHotStore = defaultRatePer1KMessageHotStore;
    this.defaultRatePer1KMessageColdStore = defaultRatePer1KMessageColdStore;
    this.defaultStoreHotMessage = defaultStoreHotMessage;
    this.defaultStoreColdMessage = defaultStoreColdMessage;
    this.defaultCosmosDBConnectionString = defaultCosmosDBConnectionString;
    this.defaultCollectionTTL = defaultCollectionTTL;
    this.defaultCollectionReservedUnits = defaultCollectionReservedUnits;
    this.defaultIoTHubConnectionString = defaultIoTHubConnectionString;
    this.defaultStorageConnectionString = defaultStorageConnectionString;
  }
}

@Component({
  selector: 'app-subscription-plan',
  templateUrl: './subscription-plan.component.html',
  styleUrls: ['./subscription-plan.component.scss']
})
export class SubscriptionPlanComponent implements OnInit {

  // Table layout config
  tableConfig = {
    'column': ['No', 'Name', 'Description', 'DefaultPlanDays', 'DefaultMaxMessageQuotaPerDay'],
    'columnClass': [8, 10, 47, 20, 25],
    'data': [],
    'canExpand': true,
    'expandTrigger': 'row'
  };

  // detail info (binding to input value in template)
  detailInfo: any;

  // unchanged detail info - use to undo edit after input change
  orginalDetailInfo: any;

  toTableCommamd = {
    'type': 'expandRow',
    'isNew': false,
    'row': {},
  };

  currentRow: any;

  constructor(private apiService: CdsApiService, private sharedService: DataShareService) {
    this.detailInfo = new EditInputInfo();
  }

  /**
   * Trigger Table Commamd
   */
  triggerTableCommand() {
    this.toTableCommamd = Object.assign({}, this.toTableCommamd);
  }

  /**
	 * updateTableContent: Trigger table update
	 * @param updatetableObject: update table object
	 */
  updateTableContent(updatetableObject) {
    this.tableConfig = Object.assign({}, updatetableObject);
  }

  /**
	 * selectRowUpdate: Select table row event
	 * @param selectRowInfo: select row info
	 */
  selectRowUpdate(selectRowInfo) {

    this.currentRow = selectRowInfo;
    this.cancelNewPanel();

    this.apiService.getSubscriptionPlan(parseInt(selectRowInfo.Id, 10)).subscribe(
      result => {

        this.detailInfo = new EditInputInfo(
          result.Name,
          result.Description,
          result.DefaultPlanDays,
          result.DefaultMaxMessageQuotaPerDay,
          result.DefaultRatePer1KMessageIngestion,
          result.DefaultRatePer1KMessageHotStore,
          result.DefaultRatePer1KMessageColdStore,
          result.DefaultStoreHotMessage,
          result.DefaultStoreColdMessage,
          result.DefaultCosmosDBConnectionString,
          result.DefaultCollectionTTL,
          result.DefaultCollectionReservedUnits,
          result.DefaultIoTHubConnectionString,
          result.DefaultStorageConnectionString,
        );
        this.orginalDetailInfo = Object.assign({}, this.detailInfo);
      },
      error => {
        console.error(error);
      }
    );
  }
  /**
	 * undoAllEdit: undo all edit , recover to the staus before change
	 */
  undoAllEdit() {
    this.detailInfo = Object.assign({}, this.orginalDetailInfo);
  }

  /**
   * cancelNewPanel: Close add item panel
   */
  cancelNewPanel() {
    this.toTableCommamd.isNew = false;
    this.toTableCommamd.type = 'collpaseAllRow';
    this.triggerTableCommand();
  }

  /**
   * cancelAddItem: exit add panel
   */
  cancelAddItem() {
    this.tableConfig.data.shift();
    this.cancelNewPanel();
  }

  /**
   * deleteItem: delete specific item
   */
  deleteItem() {

    this.apiService.deleteSubscription(this.currentRow.Id).subscribe(
      result => {
        for (let i = 0; i < this.tableConfig.data.length; i++) {
          if (this.tableConfig.data[i].Id === this.currentRow.Id) {
            this.tableConfig.data.splice(i, 1);
            this.updateTableContent(this.tableConfig);
          }
        }

        this.sharedService.publishPopupWindowMsg({
          'type': 'feedback',
          'id': 'deleteSubscriptionPlanSuccess'
        });
      },
      error => {
        console.error(error);
      }
    );
  }

  /**
   * deleteConfirmDialog: open delete clients confirm panel
   */
  deleteConfirmDialog($event) {

    const posX = $event.currentTarget.offsetLeft;
    const posY = $event.currentTarget.offsetTop;

    this.sharedService.publishPopupWindowMsg({
      'type': 'confirm',
      'id': 'deleteSubscriptionPlan',
      'position': { x: posX + 255, y: posY },
      'confirmCallback': () => this.deleteItem(),
      'cancelCallback': () => null
    });
  }

  /**
   * confirmEdit: confirm edit or add item
   * @param editForm: form data
   */
  confirmEdit(editForm: NgForm) {

    if (editForm.valid) {

      const updateObj = {
        'Name': this.detailInfo.name,
        'Description': this.detailInfo.description,
        'DefaultRatePer1KMessageIngestion': this.detailInfo.defaultRatePer1KMessageIngestion,
        'DefaultRatePer1KMessageHotStore': this.detailInfo.defaultRatePer1KMessageHotStore,
        'DefaultRatePer1KMessageColdStore': this.detailInfo.defaultRatePer1KMessageColdStore,
        'DefaultPlanDays': this.detailInfo.defaultPlanDays,
        'DefaultMaxMessageQuotaPerDay': this.detailInfo.defaultMaxMessageQuotaPerDay,
        'DefaultStoreHotMessage': this.detailInfo.defaultStoreHotMessage,
        'DefaultStoreColdMessage': this.detailInfo.defaultStoreColdMessage,
        'DefaultCosmosDBConnectionString': this.detailInfo.defaultCosmosDBConnectionString,
        'DefaultCollectionTTL': this.detailInfo.defaultCollectionTTL,
        'DefaultCollectionReservedUnits': this.detailInfo.defaultCollectionReservedUnits,
        'DefaultIoTHubConnectionString': this.detailInfo.defaultIoTHubConnectionString,
        'DefaultStorageConnectionString': this.detailInfo.defaultStorageConnectionString
      };

      // Create new Item
      if (this.toTableCommamd.isNew) {

        this.apiService.addSubscriptionPlan(updateObj).subscribe(
          result => {
            this.tableConfig.data[0]['No'] = this.tableConfig.data.length;
            this.tableConfig.data[0]['Name'] = updateObj.Name;
            this.tableConfig.data[0]['Description'] = updateObj.Description;
            this.tableConfig.data[0]['DefaultPlanDays'] = updateObj.DefaultPlanDays;
            this.tableConfig.data[0]['DefaultMaxMessageQuotaPerDay'] = updateObj.DefaultMaxMessageQuotaPerDay;
            this.tableConfig.data[0]['Id'] = result.Id;
            delete this.tableConfig.data[0].new;
            this.updateTableContent(this.tableConfig);
            this.cancelNewPanel();
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'createSubscriptionPlanSuccess'
            });
          },
          error => {
            console.error(error);
          }
        );

        // Edit specific Item
      } else {
        this.apiService.editSubscriptionPlan(this.currentRow.Id, updateObj).subscribe(
          result => {
            for (let i = 0; i < this.tableConfig.data.length; i++) {
              if (this.tableConfig.data[i].Id === this.currentRow.Id) {
                this.tableConfig.data[i]['Name'] = updateObj.Name;
                this.tableConfig.data[i]['Description'] = updateObj.Description;
                this.tableConfig.data[i]['DefaultRatePer1KMessageIngestion'] = updateObj.DefaultRatePer1KMessageIngestion;
                this.tableConfig.data[i]['DefaultRatePer1KMessageHotStore'] = updateObj.DefaultRatePer1KMessageHotStore;
                this.tableConfig.data[i]['DefaultRatePer1KMessageColdStore'] = updateObj.DefaultRatePer1KMessageColdStore;
                this.tableConfig.data[i]['DefaultPlanDays'] = updateObj.DefaultPlanDays;
                this.tableConfig.data[i]['DefaultMaxMessageQuotaPerDay'] = updateObj.DefaultMaxMessageQuotaPerDay;
                this.tableConfig.data[i]['DefaultStoreHotMessage'] = updateObj.DefaultStoreHotMessage;
                this.tableConfig.data[i]['DefaultStoreColdMessage'] = updateObj.DefaultStoreColdMessage;
                this.tableConfig.data[i]['DefaultCosmosDBConnectionString'] = updateObj.DefaultCosmosDBConnectionString;
                this.tableConfig.data[i]['DefaultCollectionTTL'] = updateObj.DefaultCollectionTTL;
                this.tableConfig.data[i]['DefaultCollectionReservedUnits'] = updateObj.DefaultCollectionReservedUnits;
                this.tableConfig.data[i]['DefaultIoTHubConnectionString'] = updateObj.DefaultIoTHubConnectionString;
                this.tableConfig.data[i]['DefaultStorageConnectionString'] = updateObj.DefaultStorageConnectionString;
                this.updateTableContent(this.tableConfig);
              }
            }
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'editSubscriptionPlanSuccess'
            });
          },
          error => {
            console.error(error);
          }
        );
      }
    }
  }

  /**
   * addBtnClick: Action after click add new item button
   */
  addBtnClick() {

    if (this.toTableCommamd.isNew) {
      return false;
    }

    this.detailInfo = new EditInputInfo();
    this.tableConfig.data.unshift(
      {
        'new': true
      }
    );

    this.toTableCommamd.isNew = true;
    this.toTableCommamd.type = 'expandRow';
    this.triggerTableCommand();
  }

  /**
   * receiveCommand from child component
   * @param commandObject: command object
   */
  receiveCommand(commandObject) {

    // commamd from table component
    if (commandObject.commandType === 'tableCommand') {

      switch (commandObject.command) {

        case 1: // select row command
          this.selectRowUpdate(commandObject.data);
          break;

        case 2: // cancel add panel
          this.tableConfig.data.shift();
          this.cancelNewPanel();
          break;

        default:

      }

    }

  }

  ngOnInit() {
    // get SuperAdmin Info
    this.apiService.getAllSubscriptionPlanList().subscribe(
      result => {
        for (let i = 0; i < result.length; i++) {
          this.tableConfig.data.push({
            'No': (i + 1),
            'Name': result[i].Name,
            'Description': result[i].Description,
            'DefaultPlanDays': result[i].DefaultPlanDays,
            'DefaultMaxMessageQuotaPerDay': result[i].DefaultMaxMessageQuotaPerDay,
            'DefaultRatePer1KMessageIngestion': result[i].DefaultRatePer1KMessageIngestion,
            'DefaultRatePer1KMessageHotStore': result[i].DefaultRatePer1KMessageHotStore,
            'DefaultRatePer1KMessageColdStore': result[i].DefaultRatePer1KMessageColdStore,
            'DefaultStoreHotMessage': result[i].DefaultStoreHotMessage,
            'DefaultStoreColdMessage': result[i].DefaultStoreColdMessage,
            'DefaultCosmosDBConnectionString': result[i].DefaultCosmosDBConnectionString,
            'DefaultCollectionTTL': result[i].DefaultCollectionTTL,
            'DefaultCollectionReservedUnits': result[i].DefaultCollectionReservedUnits,
            'DefaultIoTHubConnectionString': result[i].DefaultIoTHubConnectionString,
            'DefaultStorageConnectionString': result[i].DefaultStorageConnectionString,
            'Id': result[i].Id
          });
        }
        this.updateTableContent(this.tableConfig);
      },
      error => {
        console.log('error');
      }
    );
  }
}

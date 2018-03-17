import { Component, OnInit, Input, EventEmitter } from '@angular/core';
import { CdsApiService } from '../../../../service/cds-api.service';
import { DataShareService} from '../../../../service/data-share.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';
import { TransferTimeService } from '../../../../service/transfer-time.service';

class EditInputInfo {
  subscriptionName: String = null;
  startDate = null;
  expiredDate = null;
  ratePer1KMessageColdStore: Number = null;
  ratePer1KMessageHotStore: Number = null;
  ratePer1KMessageIngestion: Number = null;
  maxMessageQuotaPerDay: Number = null;
  storeColdMessage: Boolean = null;
  storeHotMessage: Boolean = null;
  cosmosDBName: String = null;
  cosmosDBColletionId: String = null;
  cosmosDBCollectionTTL: Number = null;
  cosmosDBCollectionReservedUnits: Number = null;
  cosmosDBConnectionString: String = null;
  ioTHubConsumerGroup: String = null;
  ioTHubConnectionString: String = null;
  storageConnectionString: String = null;

  constructor(subscriptionName: String = null, startDate = null, expiredDate = null, ratePer1KMessageColdStore: Number = null,
              ratePer1KMessageHotStore: Number = null, ratePer1KMessageIngestion: Number = null, maxMessageQuotaPerDay: Number = null,
              storeColdMessage: Boolean = null, storeHotMessage: Boolean = null, cosmosDBName: String = null,
              cosmosDBColletionId: String = null, cosmosDBCollectionTTL: Number = null, cosmosDBCollectionReservedUnits: Number = null,
              cosmosDBConnectionString: String = null, ioTHubConsumerGroup: String = null, ioTHubConnectionString: String = null,
              storageConnectionString: String = null) {

                this.subscriptionName = subscriptionName;
                this.startDate = startDate;
                this.expiredDate = expiredDate;
                this.ratePer1KMessageColdStore = ratePer1KMessageColdStore;
                this.ratePer1KMessageHotStore = ratePer1KMessageHotStore;
                this.ratePer1KMessageIngestion = ratePer1KMessageIngestion;
                this.maxMessageQuotaPerDay = maxMessageQuotaPerDay;
                this.storeColdMessage = storeColdMessage;
                this.storeHotMessage = storeHotMessage;
                this.cosmosDBName = cosmosDBName;
                this.cosmosDBColletionId = cosmosDBColletionId;
                this.cosmosDBCollectionTTL = cosmosDBCollectionTTL;
                this.cosmosDBCollectionReservedUnits = cosmosDBCollectionReservedUnits;
                this.cosmosDBConnectionString = cosmosDBConnectionString;
                this.ioTHubConsumerGroup = ioTHubConsumerGroup;
                this.ioTHubConnectionString = ioTHubConnectionString;
                this.storageConnectionString = storageConnectionString;
  }
}
@Component({
  selector: 'app-edit-subscription',
  templateUrl: './edit-subscription.component.html',
  styleUrls: ['./edit-subscription.component.scss'],
  providers: [ TransferTimeService]
})
export class EditSubscriptionComponent implements OnInit {

  @Input() editObject: any;

  tableConfig = {
    'column': ['No', 'Name', 'Start Date', 'Expired Date', 'Status'],
    'columnClass': [5, 30, 25, 25, 15],
    'data': [],
    'canExpand': true,
    'expandTrigger': 'row',
    'customColumn': 'Status'
  };

  detailInfo: any;
  orginalDetailInfo: any;
  resetPasswordStatus: Boolean = false;
  currentRow: any;

  toTableCommamd = {
    'type': 'expandRow',
    'isNew': false,
    'row': {},
  };

  // all subscription
  allSubscriptionPlan = [];

  // select SubscriptionPlan Id
  selectSubscriptionPlanId = null;

  // current subscription plan is is expoired or not
  currentPlanExpired = false;

  // clinets has any active subscription or not
  hasActiveSubscription = false; 

  constructor(private apiService: CdsApiService, private sharedService: DataShareService,
              private transferTimeService: TransferTimeService) {
    this.detailInfo = new EditInputInfo();
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

    if (this.currentRow.isExpired) {
      this.currentPlanExpired = true;
    }else {
      this.currentPlanExpired = false;
    }

    this.apiService.getCompanySubscriptionInfo( parseInt(this.editObject.id, 10) , parseInt(selectRowInfo.Id, 10) ).subscribe(
      result => {

        this.selectSubscriptionPlanId = result.SubscriptionPlanId;
        this.detailInfo = new EditInputInfo(result.SubscriptionName,
                                            this.transferTimeService.transferUTCTimeToLocalTime(result.StartDate),
                                            this.transferTimeService.transferUTCTimeToLocalTime(result.ExpiredDate),
                                            result.RatePer1KMessageColdStore,
                                            result.RatePer1KMessageHotStore,
                                            result.RatePer1KMessageIngestion,
                                            result.MaxMessageQuotaPerDay,
                                            result.StoreColdMessage,
                                            result.StoreHotMessage,
                                            result.CosmosDBName,
                                            result.CosmosDBColletionId,
                                            result.CosmosDBCollectionTTL,
                                            result.CosmosDBCollectionReservedUnits,
                                            result.CosmosDBConnectionString,
                                            result.IoTHubConsumerGroup,
                                            result.IoTHubConnectionString,
                                            result.StorageConnectionString);

        this.orginalDetailInfo =  Object.assign({}, this.detailInfo);
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
    this.editObject.isNew = false;
    this.resetPasswordStatus = false;
    this.toTableCommamd.type = 'collpaseAllRow';
    this.toTableCommamd.isNew = false;
    this.triggerTableCommand();
  }

  /**
   * cancelAddItem: exit add panel
   */
  cancelAddItem() {
    this.tableConfig.data.shift();
    this.allSubscriptionPlan.length = 0;
    this.selectSubscriptionPlanId = null;
    this.cancelNewPanel();
  }

  /**
   * deleteItem: delete specific item
   */
  deleteItem() {

    const clientId = this.editObject.id;
    const expiredTime = this.getPlanTime();

    const updateObj = {
      SubscriptionPlanId: parseInt(this.selectSubscriptionPlanId, 10),
      ExpiredDate: expiredTime
    };
    this.apiService.editCompanySubscriptionPlan(clientId, this.currentRow.Id, updateObj).subscribe(
      result => {

        for (let i = 0; i < this.tableConfig.data.length; i++) {
          if (this.tableConfig.data[i].Id === this.currentRow.Id) {
            this.tableConfig.data[i]['Expired Date'] = this.transferTimeService.transferUTCTimeToLocalTime(expiredTime, true);
            this.tableConfig.data[i]['isExpired'] = true;
            this.tableConfig.data[i]['Status'] = '<div><div class="status-dot dot-expired"></div><span>Expired</span></div>';
            this.updateTableContent(this.tableConfig);
            this.hasActiveSubscription = false;
          }
        }

        this.sharedService.publishPopupWindowMsg({
          'type': 'feedback',
          'id': 'cancelClientsSubscriptionSuccess'
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
      'id': 'cancelClientsSubscription',
      'position': {x: posX + 255, y: posY},
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
      const clientId = this.editObject.id;
      const updateObj = {
        SubscriptionPlanId: parseInt(this.selectSubscriptionPlanId, 10),
        SubscriptionName: this.detailInfo.subscriptionName,
        RatePer1KMessageIngestion: this.detailInfo.ratePer1KMessageIngestion,
        RatePer1KMessageHotStore: this.detailInfo.ratePer1KMessageHotStore,
        RatePer1KMessageColdStore: this.detailInfo.ratePer1KMessageColdStore,
        StartDate: this.transferTimeService.transferLocalTimeToUTCTime(this.detailInfo.startDate),
        ExpiredDate: this.transferTimeService.transferLocalTimeToUTCTime(this.detailInfo.expiredDate),
        MaxMessageQuotaPerDay: this.detailInfo.maxMessageQuotaPerDay,
        StoreHotMessage: this.detailInfo.storeHotMessage,
        StoreColdMessage: this.detailInfo.storeColdMessage,
        CosmosDBConnectionString: this.detailInfo.cosmosDBConnectionString,
        CosmosDBName: this.detailInfo.cosmosDBName,
        CosmosDBColletionId: this.detailInfo.cosmosDBColletionId,
        CosmosDBCollectionTTL: this.detailInfo.cosmosDBCollectionTTL,
        CosmosDBCollectionReservedUnit: this.detailInfo.cosmosDBCollectionReservedUnits,
        IoTHubConnectionString: this.detailInfo.ioTHubConnectionString,
        IoTHubConsumerGroup: this.detailInfo.ioTHubConsumerGroup,
        StorageConnectionString: this.detailInfo.storageConnectionString,
      };

      const newPlanExpired = this.detectExpired(updateObj.ExpiredDate);

      if (newPlanExpired) {
        this.hasActiveSubscription = false;
      }else {
        this.hasActiveSubscription = true;
      }

      // Create new item
      if (this.editObject.isNew) {

        this.apiService.addCompanySubscriptionPlan(clientId, updateObj).subscribe(
          result => {
            this.tableConfig.data[0]['No'] = this.tableConfig.data.length;
            this.tableConfig.data[0]['Name'] = updateObj.SubscriptionName;
            this.tableConfig.data[0]['Start Date'] = this.transferTimeService.transferUTCTimeToLocalTime(updateObj.StartDate, true);
            this.tableConfig.data[0]['Expired Date'] = this.transferTimeService.transferUTCTimeToLocalTime(updateObj.ExpiredDate, true);
            this.tableConfig.data[0]['Id'] = result.Id;
            this.tableConfig.data[0]['isExpired'] = newPlanExpired;

            if (newPlanExpired) {
              this.tableConfig.data[0]['Status'] = '<div><div class="status-dot dot-expired"></div>' +
              '<span>Expired</span></div>';
            }else {
              this.tableConfig.data[0]['Status'] = '<div><div class="status-dot dot-active"></div>' +
              '<span>Active</span></div>';
            }

            delete this.tableConfig.data[0].new;
            this.updateTableContent(this.tableConfig);
            this.cancelNewPanel();

            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'createClientsSubscriptionSuccess'
            });
          },
          error => {
              console.error(error);
          }
        );

      // Edit specific item
      }else {
        this.apiService.editCompanySubscriptionPlan(clientId, this.currentRow.Id, updateObj).subscribe(
          result => {
            for (let i = 0; i < this.tableConfig.data.length; i++) {
              if (this.tableConfig.data[i].Id === this.currentRow.Id) {
                this.tableConfig.data[i]['Expired Date'] = this.transferTimeService.transferUTCTimeToLocalTime(updateObj.ExpiredDate, true);
                this.tableConfig.data[i]['isExpired'] = newPlanExpired;

                if (newPlanExpired) {
                  this.tableConfig.data[i]['Status'] = '<div><div class="status-dot dot-expired"></div>' +
                  '<span>Expired</span></div>';
                }

                this.updateTableContent(this.tableConfig);
              }
            }
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'editClientsSubscriptionSuccess'
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

    this.currentPlanExpired = false;
    this.editObject.isNew = true;
    this.selectSubscriptionPlanId = null;
    this.detailInfo = new EditInputInfo();
    this.tableConfig.data.unshift(
      {
        'new': true
      }
    );
    this.toTableCommamd.isNew = true;
    this.toTableCommamd.type = 'expandRow';
    this.triggerTableCommand();

    this.apiService.getAllSubscriptionPlanList().subscribe(
      result => {
        for ( let i = 0; i < result.length; i++) {
          this.allSubscriptionPlan.push({
            name: result[i].Name,
            period: result[i].DefaultPlanDays,
            description: result[i].Description,
            id: result[i].Id
          });
        }
      },
      error => {
          console.error(error);
      }
    );

  }

  triggerTableCommand() {
    this.toTableCommamd = Object.assign({}, this.toTableCommamd);
  }

  /**
   * receiveCommand from child component
   * @param commandObject: command object
   */
  receiveCommand(commandObject) {

    // commamd from table component
    if (commandObject.commandType === 'tableCommand') {

      switch (commandObject.command) {

        case 1 : // select row command
          this.selectRowUpdate(commandObject.data);
          break;

        case 2 : // cancel add Item
          this.tableConfig.data.shift();
          this.cancelNewPanel();
          break;

        default:

      }

    }

  }
  /**
   * getPlanTime : Get subscription plan day (utc time -> local time)
   * @param period : (optional) period day
   */
  getPlanTime(period = null) {

    let UTCTime = this.transferTimeService.getTime(true);

    if (period) {
      UTCTime = UTCTime.add(period, 'days');
    }
    return this.transferTimeService.transferUTCTimeToLocalTime(UTCTime);
  }
  /**
   * detectExpired: detect is expired or not
   * @param expiredTime: expire time
   */
  detectExpired(expiredTime) {
    let isExpired: Boolean = false;
    const currentTime = this.transferTimeService.getTime();
    const planExpiredTime = this.transferTimeService.getTime(false, expiredTime);

    if (planExpiredTime.isSameOrBefore(currentTime)) {
      isExpired = true;
    }else {
      isExpired = false;
    }
    return isExpired;
  }

  /**
   * selectSubscription: After select new subscription
   * @param planeId : subscription plan Id
   */
  selectSubscription(planeId) {

    this.selectSubscriptionPlanId = planeId;

    this.apiService.getSubscriptionPlan( planeId).subscribe(
      result => {
        this.detailInfo = new EditInputInfo(result.Name,
                                            this.getPlanTime(), // now
                                            this.getPlanTime(result.DefaultPlanDays), // days after
                                            result.DefaultRatePer1KMessageColdStore,
                                            result.DefaultRatePer1KMessageHotStore,
                                            result.DefaultRatePer1KMessageIngestion,
                                            result.DefaultMaxMessageQuotaPerDay,
                                            result.DefaultStoreColdMessage,
                                            result.DefaultStoreHotMessage,
                                            null,
                                            null,
                                            result.DefaultCollectionTTL,
                                            result.DefaultCollectionReservedUnits,
                                            result.DefaultCosmosDBConnectionString,
                                            null,
                                            result.DefaultIoTHubConnectionString,
                                            result.DefaultStorageConnectionString);
      },
      error => {
          console.error(error);
      }
    );
  }

  ngOnInit() {

    let hasSubscription = false;

    this.apiService.getCompanySubscriptionList(this.editObject.id).subscribe(
      result => {

        for (let i = 0; i < result.length; i++) {

          const isExpired = this.detectExpired(result[i].ExpiredDate);

          if (isExpired === false) {
            hasSubscription = true;
          }

          const status = (isExpired) ? '<div><div class="status-dot dot-expired"></div><span>Expired</span></div>' :
                                       '<div><div class="status-dot dot-active"></div><span>Active</span></div>';

          this.tableConfig.data.push({
            'No': (i + 1),
            'Name': result[i].SubscriptionName,
            'Start Date': this.transferTimeService.transferUTCTimeToLocalTime(result[i].StartDate, true),
            'Expired Date': this.transferTimeService.transferUTCTimeToLocalTime(result[i].ExpiredDate, true),
            'Status': status,
            'Id': result[i].Id,
            'isExpired': isExpired
          });
        }

        if (hasSubscription) {
          this.hasActiveSubscription = true;
        }

        this.updateTableContent(this.tableConfig);
      },
      error => {
          console.error(error);
      }
    );
  }
}

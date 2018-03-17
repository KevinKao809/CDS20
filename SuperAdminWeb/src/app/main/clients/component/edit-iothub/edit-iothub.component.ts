import { Component, OnInit, OnDestroy, Input, EventEmitter, ViewChild} from '@angular/core';
import { CdsApiService } from '../../../../service/cds-api.service';
import { DataShareService } from '../../../../service/data-share.service';
import { TransferTimeService } from '../../../../service/transfer-time.service';
import { SignalrService } from '../../../../service/signalr.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';
import { Subscription } from 'rxjs/subscription';

import { ExpandableTableComponent } from '../../../shared/expandable-table/expandable-table.component';

@Component({
  selector: 'app-edit-iothub',
  templateUrl: './edit-iothub.component.html',
  styleUrls: ['./edit-iothub.component.scss'],
  providers: [SignalrService, TransferTimeService]
})
export class EditIothubComponent implements OnInit, OnDestroy {

  @Input() editObject: any;
  @ViewChild(ExpandableTableComponent) expandableTableComponent;

  tableConfig = {
    'column': ['No', 'Alias', 'Description', 'Status', 'Updated Time', 'Action'],
    'columnClass': [5, 20, 20, 10, 20, 25],
    'data': [],
    'canExpand': true,
    'expandTrigger': 'button',
    'customizedColumn': 'Action'
  };

  detailInfo: any;
  orginalDetailInfo: any;
  toTableCommamd = {
    'type': 'expandRow',
    'isNew': false,
    'row': {},
  };

  // Switch component id
  iotHubExpandContentId = '';

  // expand detail panel info
  secondLayerObj = {
    'clientId': '',
    'id': '',
    'isNew': false
  };

  // time interval checking
  checkIotHubTimeInterval = 10000;

  // time interval checking timer
  checkTimer = null;

  iotHubCheckTimer = {};

  iotHubUdateStatus = {};

  iotHubUpdatedData = {};

  private signalrSubscription: Subscription;

  constructor(private apiService: CdsApiService,
              private sharedService: DataShareService,
              private signalrService: SignalrService,
              private transferTimeService: TransferTimeService) {}

  /**
   * receivedRealTimeMsg: received realtime message from signalR Hub
   * @param realTimeMsg : realtime message received from signalr
   */
  receivedRealTimeMsg(realTimeMsg) {

    if (realTimeMsg && realTimeMsg.topic === 'Process Heartbeat') {
      console.log('Process Heartbeat:' , realTimeMsg);

      const iotHubId = parseInt(realTimeMsg.companyId, 10);

      if (this.editObject.id === parseInt(realTimeMsg.companyId, 10)) {
        this.updateTabeHeartBeatStatus(realTimeMsg.timestampSource, iotHubId);
        this.iotHubUpdatedData = realTimeMsg;
        this.iotHubUdateStatus[realTimeMsg.companyId] = true;
      }
    }
  }

  /**
   * updateTabeHeartBeatStatus: change table content after received signalr message
   * @param updateTime update time
   * @param iotHubId iot Hub id
   */
  updateTabeHeartBeatStatus(updateTime, iotHubId) {
    const updaeLocalTime = this.transferTimeService.transferUTCTimeToLocalTime(updateTime, true, 'YYYY-MM-DD HH:mm:ss');

    for (let i = 0; i < this.tableConfig.data.length; i++) {
      if (this.tableConfig.data[i].Id === iotHubId) {
        const updatStatus = '<div><div class="status-dot dot-active"></div><span>Running</span></div>';

        this.tableConfig.data[i]['Status'] = updatStatus;
        this.tableConfig.data[i]['Updated Time'] = updaeLocalTime;
        this.expandableTableComponent.directUpdateCell({
          id: iotHubId,
          update: {
            'Status' : updatStatus,
            'UpdatedTime': updaeLocalTime
          }
        });
        break;
      }
    }
  }

  startTimeIntervalCheck(iotHubId) {

    this.iotHubCheckTimer[iotHubId] =  setInterval(() => {

      // do received signalr message durning period
      if (this.iotHubUdateStatus[iotHubId]) {
        this.resetTimeInterval(iotHubId); // Rest timeout

        // no signalr input durning period
      }else {

        for (let i = 0; i < this.tableConfig.data.length; i++) {
          if (this.tableConfig.data[i].Id === iotHubId) {
            const updatStatus = '<div><div class="status-dot"></div><span>Shutdown</span></div>';
            this.tableConfig.data[i]['Status'] = updatStatus;
            this.expandableTableComponent.directUpdateCell({
              id: iotHubId,
              update: {
                'Status' : updatStatus,
                // 'UpdatedTime': updaeLocalTime
              }
            });

            break;
          }
        }


      }

      this.iotHubUdateStatus[iotHubId] = false;

    } , this.checkIotHubTimeInterval);
  }

  resetTimeInterval(iotHubId) {
    clearInterval(this.iotHubCheckTimer[iotHubId]);
    this.startTimeIntervalCheck(iotHubId);
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
    this.secondLayerObj.id = selectRowInfo.Id;
    this.toTableCommamd.row = selectRowInfo;
  }

  /**
   * cancelNewPanel: Close add item panel
   */
  cancelNewPanel() {
    this.tableConfig.data.splice(0, 1);
    this.editObject.isNew = false;
    this.toTableCommamd.type = 'collpaseAllRow';
    this.toTableCommamd.isNew = false;
    this.triggerTableCommand();
  }

  /**
   * addBtnClick: Action after click add new item button
   */
  addBtnClick() {

    if (this.toTableCommamd.isNew) {
      return false;
    }

    this.editObject.isNew = true;
    this.tableConfig.data.unshift(
      {
        'new': true
      }
    );
    this.toTableCommamd.row = {};
    this.iotHubExpandContentId = 'iotHubDetail';
    this.secondLayerObj.isNew = true;
    this.toTableCommamd.type = 'expandRow';
    this.toTableCommamd.isNew = true;
    this.triggerTableCommand();
  }

  triggerTableCommand() {
    this.toTableCommamd = Object.assign({}, this.toTableCommamd);
  }

  changeExpandContent(contentId: Number) {

    if (contentId === 0) {
      this.iotHubExpandContentId = 'iotHubDetail';
    }else if (contentId === 1) {
      this.iotHubExpandContentId = 'iotHubMonitor';
    }

    this.secondLayerObj.isNew = false;
    this.toTableCommamd.type = 'expandRow';
    this.toTableCommamd.isNew = false;

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

        case 0 : // select row command
          this.selectRowUpdate(commandObject.data);
        break;

        case 2 : // cancel add item
          this.cancelNewPanel();
          break;

        default:

      }

    }else if (commandObject.commandType === 'iotHubCommand') {

      switch (commandObject.command) {

        case 0 : // create success
          delete this.tableConfig.data[0].new;
          this.sharedService.publishPopupWindowMsg({
            'type': 'feedback',
            'id': 'createIotHubSuccess'
          });
          this.toTableCommamd.type = 'collapseAllRow';
          this.triggerTableCommand();
          break;

        case 1 : // edit success
          this.updateTableContent(commandObject.data);
          this.sharedService.publishPopupWindowMsg({
            'type': 'feedback',
            'id': 'editIotHubSuccess'
          });
          this.toTableCommamd.type = 'collapseAllRow';
          this.triggerTableCommand();
          break;
        case 2 : // delete client success
          this.updateTableContent(commandObject.data);
          this.sharedService.publishPopupWindowMsg({
            'type': 'feedback',
            'id': 'deleteteIotHubSuccess'
          });
          this.toTableCommamd.type = 'collapseAllRow';
          this.triggerTableCommand();
          break;

        case 3 : // cancel client create panel
          this.cancelNewPanel();
          break;

        default:

      }
    }

  }

  ngOnDestroy() {
    this.signalrService.hubDisconnect();
    this.signalrSubscription.unsubscribe();
    // clear all timer
    Object.keys(this.iotHubCheckTimer).forEach(function(key) {
      clearInterval(this.iotHubCheckTimer[key]);
    }.bind(this));
  }

  ngOnInit() {

    this.secondLayerObj.clientId = this.editObject.id;
    this.signalrService.hubConnect();
    this.signalrSubscription = this.sharedService.signalrMsg$.subscribe(msg => {
      const realTimeMsg = JSON.parse(msg);
      this.receivedRealTimeMsg(realTimeMsg);
    });

    this.apiService.getIotHubList(this.editObject.id).subscribe(
      result => {
        for (let i = 0; i < result.length; i++) {
          this.tableConfig.data.push({
            'No': (i + 1),
            'Alias': result[i].IoTHubName,
            'Description': result[i].Description,
            'Status': 'checking...',
            'Id': result[i].Id,
            'Updated Time': null
          });
          this.iotHubUdateStatus[result[i].Id] = false;
          this.startTimeIntervalCheck(result[i].Id);
        }
        this.updateTableContent(this.tableConfig);
      },
      error => {
          console.error(error);
      }
    );

  }

}

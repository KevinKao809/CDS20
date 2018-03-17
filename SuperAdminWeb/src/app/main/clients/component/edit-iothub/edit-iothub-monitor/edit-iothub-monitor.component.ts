import { Component, OnInit, OnChanges, Input, ComponentFactoryResolver, ViewContainerRef, ViewChild } from '@angular/core';
import { IotHubMonitorBoardComponent } from '../iot-hub-monitor-board/iot-hub-monitor-board.component';
import { TransferTimeService } from '../../../../../service/transfer-time.service';

@Component({
  selector: 'app-edit-iothub-monitor',
  templateUrl: './edit-iothub-monitor.component.html',
  styleUrls: ['./edit-iothub-monitor.component.scss'],
  providers: [TransferTimeService]
})
export class EditIothubMonitorComponent implements OnInit, OnChanges {

  @Input() realTimeData: any = {};
  @Input() monitorObject: any;
  @ViewChild('dynamicInsert', { read: ViewContainerRef }) dynamicInsert: ViewContainerRef;

  haveDataToShow: Boolean = false;

  iotHubRecriverRecord = {};

  constructor(private componentFactoryResolver: ComponentFactoryResolver, private transferTimeService: TransferTimeService) { }

  // Update Iot Hub Receiver monitor Widget
  updateMonitorBoard(updateData) {
    this.iotHubRecriverRecord[updateData['partition']].receivedMessage = updateData;
  }

  // Create Iot Hub Receiver monitor Widget
  createMonitorBoard(createData) {
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(IotHubMonitorBoardComponent);
    this.iotHubRecriverRecord[createData['partition']] =
    <IotHubMonitorBoardComponent>this.dynamicInsert.createComponent(componentFactory).instance;

    this.iotHubRecriverRecord[createData['partition']].receivedMessage = createData;
  }

  dataParsing(rawData) {

    console.log(rawData.status);

    if (rawData.status.includes('Running')) {
      if (rawData.status.includes('Good')) {
        rawData.runningStatus = 'good';
      }else {
        rawData.runningStatus = 'error';
      }
    }else {
      rawData.runningStatus = 'shoutDown';
    }

    rawData.cpu = parseInt(rawData.cpu, 10);
    rawData.messageConsumedDate = this.transferTimeService.transferUTCTimeToLocalTime(rawData.messageConsumedDate,
                                                                                      true, 'YYYY-MM-DD HH:mm:ss');
    rawData.timestampSource = this.transferTimeService.transferUTCTimeToLocalTime(rawData.timestampSource,
                                                                                  true, 'YYYY-MM-DD HH:mm:ss');

    return rawData;
  }

  ngOnChanges() {

    const realTimeMessageClientId = parseInt(this.realTimeData.companyId, 10);
    const realTimeMessageIotHubId = parseInt(this.realTimeData.iotHubId, 10);

    if (this.monitorObject.clientId === realTimeMessageClientId && this.monitorObject.id === realTimeMessageIotHubId) {
      this.haveDataToShow = true;
      const parseRealTimeData = this.dataParsing(this.realTimeData);

      // Already exits this widget - update
      if (this.iotHubRecriverRecord[parseRealTimeData.partition]) {

        this.updateMonitorBoard(parseRealTimeData);

      // Create new widget
      }else {
        this.createMonitorBoard(parseRealTimeData);
      }
    }
  }

  ngOnInit() {
  }

}

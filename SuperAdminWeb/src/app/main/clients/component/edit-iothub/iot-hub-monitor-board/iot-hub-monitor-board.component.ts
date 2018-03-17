import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-iot-hub-monitor-board',
  templateUrl: './iot-hub-monitor-board.component.html',
  styleUrls: ['./iot-hub-monitor-board.component.scss']
})
export class IotHubMonitorBoardComponent {

  @Input('receivedMessage') receivedMessage;

  gaugeType = 'semi';
  gaugeLabel = '';
  gaugeAppendText = '%';

  thresholdConfig = {
    '0': {color: '#00a651'},
    '50': {color: 'orange'},
    '75': {color: '#d83801'}
  };

  constructor() { }

}

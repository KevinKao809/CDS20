import { Injectable } from '@angular/core';
import { DataShareService } from './data-share.service';

declare var $: any;

@Injectable()
export class SignalrService {

  private connection: any;
  private hub: any;
  private hubUrl: any = 'http://msghub20.dev.iot-cds.net/signalr';

  constructor(private dataShareService: DataShareService) { }

  hubConnect() {

    this.connection = $.connection;
    this.connection.hub.url = this.hubUrl;
    this.hub = $.connection.RTMessageHub;

    this.hub.client.onReceivedMessage = function (message) {
      console.log(message);
      this.dataShareService.publishSignalRMsg(message);
    }.bind(this);

    this.connection.hub.start({ withCredentials: false }).done(function () {
      this.hub.server.register(0);
    }.bind(this));
  }

  hubDisconnect() {
    this.connection.hub.stop();
  }

}

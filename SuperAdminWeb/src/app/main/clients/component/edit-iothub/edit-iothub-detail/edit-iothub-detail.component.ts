import { Component, OnInit, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { CdsApiService } from '../../../../../service/cds-api.service';
import { DataShareService } from '../../../../../service/data-share.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';

class EditInputInfo {
  ioTHubName: String;
  description: String;
  ioTHubEndPoint: String;
  ioTHubConnectionString: String;
  eventConsumerGroup: String;
  eventHubStorageConnectionString: String;
  uploadContainer: String;
  id: Number;

  constructor(ioTHubName: String = null, description: String = null, ioTHubEndPoint: String = null, ioTHubConnectionString: String = null,
              eventConsumerGroup: String = null, eventHubStorageConnectionString: String = null, uploadContainer: String = null,
              id: Number = null) {
    this.ioTHubName = ioTHubName;
    this.description = description;
    this.ioTHubEndPoint = ioTHubEndPoint;
    this.ioTHubConnectionString = ioTHubConnectionString;
    this.eventConsumerGroup = eventConsumerGroup;
    this.eventHubStorageConnectionString = eventHubStorageConnectionString;
    this.uploadContainer = uploadContainer;
    this.id = id;
  }
}

@Component({
  selector: 'app-edit-iothub-detail',
  templateUrl: './edit-iothub-detail.component.html',
  styleUrls: ['./edit-iothub-detail.component.scss']
})
export class EditIothubDetailComponent implements OnInit {

  @Input() editObject;
  @Input() tableObject;
  @Input() refCultureList;
  @Output() toIotHubCommand = new EventEmitter();

  detailInfo: any;
  orginalDetailInfo: any;

  constructor(private apiService: CdsApiService, private dataShareService: DataShareService) {
    this.detailInfo = new EditInputInfo();
  }

  /**
   * deleteItem: Delete specific Item
   */
  deleteItem() {

    this.apiService.deleteIotHub(this.detailInfo.id).subscribe(
      result => {
        for (let i = 0; i < this.tableObject.data.length; i++) {
          if (this.tableObject.data[i].Id === this.detailInfo.id) {
            this.tableObject.data.splice(i, 1);
            break;
          }
        }
        this.toIotHubCommand.emit({
          commandType: 'iotHubCommand',
          command: 2,
          data: this.tableObject
        });
      },
      error => {

      }
    );
  }

  /**
   * cancelAddItem: Cancel add  panel
   */
  cancelAddItem() {
    this.toIotHubCommand.emit({
      commandType: 'iotHubCommand',
      command: 3,
    });
  }

  /**
   * deleteConfirmDialog: open delete clients confirm panel
   */
  deleteConfirmDialog($event) {

    const posX = $event.currentTarget.offsetLeft;
    const posY = $event.currentTarget.offsetTop;

    this.dataShareService.publishPopupWindowMsg({
      'type': 'confirm',
      'id': 'deleteIotHub',
      'position': {x: posX + 255, y: posY},
      'confirmCallback': () => this.deleteItem(),
      'cancelCallback': () => null
    });

  }

  /**
   * confirmEditClients: Save edit clients info
   * param editForm : form infomation
   */
  confirmEdit(editForm: NgForm) {

    // if form valid
    if (editForm.valid) {
      const iotHubId = this.editObject.id;
      const updateObj = {
        'IoTHubName': this.detailInfo.ioTHubName,
        'Description': this.detailInfo.description,
        'IoTHubEndPoint': this.detailInfo.ioTHubEndPoint,
        'IoTHubConnectionString': this.detailInfo.ioTHubConnectionString,
        'EventConsumerGroup': this.detailInfo.eventConsumerGroup,
        'EventHubStorageConnectionString': this.detailInfo.eventHubStorageConnectionString,
        'UploadContainer': this.detailInfo.uploadContainer
      };

      // Add New item
      if (this.editObject.isNew) {

        this.apiService.addIotHub(this.editObject.clientId, updateObj).subscribe(
          result => {
            this.tableObject.data[0]['No'] = this.tableObject.data.length;
            this.tableObject.data[0]['Alias'] = updateObj.IoTHubName;
            this.tableObject.data[0]['Description'] = updateObj.Description;
            this.tableObject.data[0]['Status'] = 'checking...';
            this.tableObject.data[0]['Id'] = result.Id;
            this.toIotHubCommand.emit({
              commandType: 'iotHubCommand',
              command: 0
            });
          },
          error => {

          }
        );


      // Edit item
      } else {

        this.apiService.editIotHubInfo(iotHubId, updateObj).subscribe(
          result => {

            this.toIotHubCommand.emit({
              commandType: 'iotHubCommand',
              command: 0
            });

            for (let i = 0; i < this.tableObject.data.length; i++) {
              if (this.tableObject.data[i].Id === this.detailInfo.id) {
                this.tableObject.data[i]['Alias'] = updateObj.IoTHubName;
                this.tableObject.data[i]['Description'] = updateObj.Description;
                break;
              }
            }

            this.toIotHubCommand.emit({
              commandType: 'iotHubCommand',
              command: 1,
              data: this.tableObject
            });

          },
          error => {
              console.error(error);
          }
        );

      }

    } else {
      console.log('format error!');
    }

  }

  /**
   * undoAllEdit: undo all edit , recover to the staus before change
   */
  undoAllEdit() {
    this.detailInfo = Object.assign({}, this.orginalDetailInfo);
  }

  ngOnInit() {
    if (!this.editObject.isNew) {
      this.apiService.getIotHubInfo(parseInt(this.editObject.id, 10) ).subscribe(
        result => {
          this.detailInfo = new EditInputInfo(result.IoTHubName, result.Description, result.IoTHubEndPoint, result.IoTHubConnectionString,
            result.EventConsumerGroup, result.EventHubStorageConnectionString, result.UploadContainer, this.editObject.id);
          this.orginalDetailInfo =  Object.assign({}, this.detailInfo);
        },
        error => {
            console.error(error);
        }
      );
    }

  }

}

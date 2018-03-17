import { Component, OnInit, Input, EventEmitter } from '@angular/core';
import { CdsApiService } from '../../../../../service/cds-api.service';
import { DataShareService} from '../../../../../service/data-share.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';

class EditInputInfo {
  name: String;
  dataType: String;
  description: String;
  defaultValue: String;
  id: String;
  constructor(name: String = null, dataType: String = 'string', description: String = null,
              defaultValue: String = null, id: String = null) {
    this.name = name;
    this.dataType = dataType;
    this.description = description;
    this.defaultValue = defaultValue;
  }
}

@Component({
  selector: 'app-device-configuration',
  templateUrl: './device-configuration.component.html',
  styleUrls: ['./device-configuration.component.scss']
})
export class DeviceConfigurationComponent implements OnInit {
  // Table layout config
  tableConfig = {
    'column': ['No', 'Name', 'Data Type', 'Description', 'Default Value'],
    'columnClass': [5, 25, 20, 30, 20],
    'data': [],
    'canExpand': true,
    'expandTrigger': 'row'
  };


  // detail info (binding to input value in template)
  detailInfo: any;

  // unchanged detail info - use to undo edit after input change
  orginalDetailInfo: any;

  defaultValue = {
    'bool' : true,
    'numeric': 'please input the number',
    'string': '0.6.7',
    'datetime': 'yyyy/mm/dd',
    'isDateTime': false,
    'Btn': 1
  };

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

    this.apiService.getDeviceConfiguration(parseInt(selectRowInfo.Id, 10)).subscribe(
      result => {
        this.detailInfo = new EditInputInfo(result.Name,
                                            result.DataType,
                                            result.Description,
                                            result.DefaultValue,
                                            result.Id);
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

    this.apiService.deleteDeviceConfiguration(this.currentRow.Id).subscribe(
      result => {
        for (let i = 0; i < this.tableConfig.data.length; i++) {
          if (this.tableConfig.data[i].Id === this.currentRow.Id) {
            this.tableConfig.data.splice(i, 1);
            this.updateTableContent(this.tableConfig);
          }
        }

        this.sharedService.publishPopupWindowMsg({
          'type': 'feedback',
          'id': 'deleteDeviceConfigurationSuccess'
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
      'id': 'deleteDeviceConfiguration',
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

      const updateObj = {
        'Name': this.detailInfo.name,
        'Description': this.detailInfo.description,
        'DataType': this.detailInfo.dataType,
        'DefaultValue': this.detailInfo.defaultValue
      };

      // Create new Item
      if (this.toTableCommamd.isNew) {

        this.apiService.addDeviceConfiguration(updateObj).subscribe(
          result => {
            this.tableConfig.data[0]['No'] = this.tableConfig.data.length;
            this.tableConfig.data[0]['Name'] = updateObj.Name;
            this.tableConfig.data[0]['Data Type'] = updateObj.DataType;
            this.tableConfig.data[0]['Description'] = updateObj.Description;
            this.tableConfig.data[0]['Default Value'] = updateObj.DefaultValue;
            this.tableConfig.data[0]['Id'] = result.Id;
            delete this.tableConfig.data[0].new;
            this.updateTableContent(this.tableConfig);
            this.cancelNewPanel();
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'createDeviceConfigurationSuccess'
            });
          },
          error => {
              console.error(error);
          }
        );

      // Edit specific Item
      }else {

        this.apiService.editDeviceConfiguration(this.currentRow.Id, updateObj).subscribe(
          result => {
            for (let i = 0; i < this.tableConfig.data.length; i++) {
              if (this.tableConfig.data[i].Id === this.currentRow.Id) {
                this.tableConfig.data[i]['Name'] = updateObj.Name;
                this.tableConfig.data[i]['Data Type'] = updateObj.DataType;
                this.tableConfig.data[i]['Description'] = updateObj.Description;
                this.tableConfig.data[i]['Default Value'] = updateObj.DefaultValue;
                this.updateTableContent(this.tableConfig);
              }
            }
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'editDeviceConfigurationSuccess'
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

        case 1 : // select row command
          console.log(commandObject);
          this.selectRowUpdate(commandObject.data);
          break;

        case 2 : // cancel add panel
          this.tableConfig.data.shift();
          this.cancelNewPanel();
          break;

        default:
      }
    }
  }

  dataTypeChange(selectDataType) {
    this.detailInfo.defaultValue = this.defaultValue[selectDataType];

    console.log(this.detailInfo.dataType);

    if (selectDataType === 'datetime') {
      this.defaultValue.isDateTime = true;
    }else {
      this.defaultValue.isDateTime = false;
    }
  }
  ngOnInit() {
    // get SuperAdmin Info
    this.apiService.getDeviceConfigurationList().subscribe(
      result => {
        for (let i = 0; i < result.length; i++) {
          this.tableConfig.data.push({
            'Id': result[i].Id,
            'No': (i + 1),
            'Name': result[i].Name,
            'Data Type': result[i].DataType,
            'Description': result[i].Description,
            'Default Value': result[i].DefaultValue
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

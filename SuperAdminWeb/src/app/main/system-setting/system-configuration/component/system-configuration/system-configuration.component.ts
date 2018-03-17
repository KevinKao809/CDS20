import { Component, OnInit } from '@angular/core';
import { CdsApiService } from '../../../../../service/cds-api.service';
import { DataShareService} from '../../../../../service/data-share.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';

class EditInputInfo {
  value: String;
  constructor(value: String = null) {
    this.value = value;
  }
}

@Component({
  selector: 'app-system-configuration',
  templateUrl: './system-configuration.component.html',
  styleUrls: ['./system-configuration.component.scss']
})
export class SystemConfigurationComponent implements OnInit {

  systemConfigGroup = ['Azure Account' , 'AdminWeb', 'API Service', 'Cosmos DB', 'Log',
                       'Message Hub', 'Redis Cache', 'Service Bus', 'Storage Account', 'Service Fabric'];

  currentGroup = null;

  tableConfig = {
    'column': ['No', 'Item', 'Value', 'Action'],
    'columnClass': [5, 30, 30, 35],
    'data': [],
    'customizedColumn': 'Action',
    'canExpand': false,
    'inlineEdit': ['Value']
  };

  toTableCommamd: any;

  // detail info (binding to input value in template)
  detailInfo: any;

  // unchanged detail info - use to undo edit after input change
  orginalDetailInfo: any;

  currentRow: any;

  constructor(private cdsApiService: CdsApiService, private sharedService: DataShareService) {
    this.detailInfo = new EditInputInfo();
   }

  selectGroup(groupName) {
    console.log(groupName);
    this.currentGroup = groupName;
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
    this.detailInfo = new EditInputInfo(selectRowInfo.Value);
    this.orginalDetailInfo = Object.assign({}, this.detailInfo);
  }

  undoAllEdit() {
    this.toTableCommamd = {
      'type': 'inlineditUndo',
      'originalValue': this.orginalDetailInfo.value
    };
  }

  confirmEdit() {

    const updateObj = {
      'Key': this.currentRow.Item,
      'Group': this.currentRow.Group,
      'Value': this.detailInfo.value,
    };

    this.cdsApiService.editSystemConfiguration(this.currentRow.Id, updateObj).subscribe(
      result => {
        for (let i = 0; i < this.tableConfig.data.length; i++) {
          if (this.tableConfig.data[i].Id === this.currentRow.Id) {
            this.tableConfig.data[i]['Value'] = updateObj.Value;

            this.toTableCommamd = {
              'type': 'cancelSelect',
            };

            break;
          }
        }
        this.sharedService.publishPopupWindowMsg({
          'type': 'feedback',
          'id': 'editSystemConfigurationSuccess'
        });
      },
      error => {
        console.log('error');
      }
    );


  }

  /**
   * receiveCommand from child component
   * @param commandObject: command object
   */
  receiveCommand(commandObject) {

    // commamd from table component
    if (commandObject.commandType === 'tableCommand') {

      switch (commandObject.command) {

        case 0: // select row command
          this.selectRowUpdate(commandObject.data);
          break;

        case 2: // cancel add panel
          // this.tableConfig.data.shift();
          // this.cancelNewPanel();
          break;

          case 3: // inline value change
            this.detailInfo = new EditInputInfo(commandObject.data);
          break;

        default:

      }
    }
  }

  ngOnInit() {

    this.cdsApiService.getSystemConfiguration().subscribe(
      result => {
        console.log(result);

        for (let i = 0; i < result.length; i++) {

          this.tableConfig.data.push({
            'No': (i + 1),
            'Item': result[i].Key,
            'Value': result[i].Value,
            'Group': result[i].Group,
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

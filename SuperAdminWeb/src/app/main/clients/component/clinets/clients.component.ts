import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CdsApiService } from '../../../../service/cds-api.service';
import { DataShareService} from '../../../../service/data-share.service';

@Component({
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.scss']
})

export class ClientsComponent implements OnInit {

  secondLayerBtnID: Number;

  toTableCommamd = {
    'type': 'expandRow',
    'isNew': false,
    'row': {},
  };

  // refCultureInfo
  refCultureInfo = [];

  // expand detail panel info
  secondLayerObj = {
    'id': '',
    'isNew': false
  };

  // table setting
  tableConfig = {
    'column': ['No', 'Name', 'ShortName', 'Contact', 'Action'],
    'columnClass': [5, 15, 15, 10, 55],
    'data': [],
    'canExpand': true,
    'expandTrigger': 'button',
    'customizedColumn': 'Action'
  };

  constructor(
    private apiService: CdsApiService,
    private shareService: DataShareService
  ) {}

  /**
   * selectRowUpdate: action after received row select command
   * param rowUpdate : selection row
   */
  selectRowUpdate(rowUpdate) {
    this.secondLayerObj.id = rowUpdate.Id;
    this.toTableCommamd.row = rowUpdate;
  }

  /**
   * addBtnClick: click add company button
   *
   */
  addBtnClick($event) {

    if (this.toTableCommamd.isNew) {
      return false;
    }
    this.toTableCommamd.type = 'expandRow';
    this.toTableCommamd.row = {};
    this.toTableCommamd.isNew = true;
    this.secondLayerObj.isNew = true;
    this.tableConfig.data.unshift(
      {
        'new': true
      }
    );
    this.secondLayerBtnID = 0;
    this.triggerTableCommand();
  }


  cancelNewPanel() {
    this.tableConfig.data.splice(0, 1);
    this.toTableCommamd.isNew = false;
    this.toTableCommamd.type = 'collapseAllRow';
    this.triggerTableCommand();
  }

  /**
   * received command from child component
   * param commandObject
   */
  receiveCommand(commandObject) {

    // commamd from table component
    if (commandObject.commandType === 'tableCommand') {

      switch (commandObject.command) {

        case 0 : // select row command
          this.selectRowUpdate(commandObject.data);
          break;

          case 2 : // cancel new client panel
            this.cancelNewPanel();
          break;

        default:

      }

    }else if (commandObject.commandType === 'clientCommand') {

      switch (commandObject.command) {

        case 0 : // create client success
          delete this.tableConfig.data[0].new;
          this.shareService.publishPopupWindowMsg({
            'type': 'feedback',
            'id': 'createClientSuccess'
          });
          this.toTableCommamd.type = 'collapseAllRow';
          this.triggerTableCommand();
          break;

        case 1 : // edit client success
          this.updateTableContent(commandObject.data);
          this.shareService.publishPopupWindowMsg({
            'type': 'feedback',
            'id': 'editClientSuccess'
          });
          this.toTableCommamd.type = 'collapseAllRow';
          this.triggerTableCommand();
          break;
        case 2 : // delete client success
          this.updateTableContent(commandObject.data);
          this.shareService.publishPopupWindowMsg({
            'type': 'feedback',
            'id': 'deleteClientSuccess'
          });
          this.toTableCommamd.type = 'collapseAllRow';
          this.triggerTableCommand();
          break;

        case 3 : // cancel client create panel
        console.log('cccc');
          this.cancelNewPanel();
          break;

        default:

      }

    }

  }

  /**
   * updateTableContent: update table object to let UI updated
   * param updatetableObject
   */
  updateTableContent(updatetableObject) {
    this.tableConfig = Object.assign({}, updatetableObject);
  }

  triggerTableCommand() {
    this.toTableCommamd = Object.assign({}, this.toTableCommamd);
  }

  triggerExpand(panelId) {
    this.secondLayerBtnID = panelId;
    this.secondLayerObj.isNew = false;
    this.toTableCommamd.type = 'expandRow';
    this.toTableCommamd.isNew = false;
    this.triggerTableCommand();
  }

  ngOnInit() {

    // get refCultureInfo
    this.apiService.getRefCultureInfo().subscribe(
      result => {
        this.refCultureInfo = result;
      },
      error => {
        console.log('error');
      }

    );

    this.apiService.getCompanyList().subscribe(
      companyList => {

        const tableData = [];

        for (let i = 0; i < companyList.length; i++) {
          this.tableConfig.data.push({
            'Id': companyList[i].Id,
            'No': (i + 1),
            'Name': companyList[i].Name,
            'ShortName': companyList[i].ShortName,
            'Contact': companyList[i].ContactName,
          });
        }
        this.updateTableContent(this.tableConfig);
      },
      error => {
          console.error(error);
      }
    );

  }

}

import { Component, OnInit, Input, EventEmitter } from '@angular/core';
import { CdsApiService } from '../../../../service/cds-api.service';
import { DataShareService} from '../../../../service/data-share.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';

class EditInputInfo {
  name: String;
  url: String;
  ordering: Number;

  constructor(name: String = null, url: String = null, ordering: Number = null) {
    this.name = name;
    this.url = url;
    this.ordering = ordering;
  }
}

@Component({
  selector: 'app-edit-dashboard',
  templateUrl: './edit-dashboard.component.html',
  styleUrls: ['./edit-dashboard.component.scss']
})
export class EditDashboardComponent implements OnInit {

  @Input() editObject: any;

  tableConfig = {
    'column': ['No', 'Name', 'URL', 'Ordering'],
    'columnClass': [10, 40, 40, 10],
    'data': [],
    'canExpand': true,
    'expandTrigger': 'row'
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

  constructor(private apiService: CdsApiService, private sharedService: DataShareService) {
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

    this.apiService.getExternalDashboardInfo( parseInt(this.editObject.id, 10) , parseInt(selectRowInfo.Id, 10) ).subscribe(
      result => {
        this.detailInfo = new EditInputInfo(result.Name, result.URL, result.Ordering);
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
    this.cancelNewPanel();
  }

  /**
   * deleteItem: delete specific item
   */
  deleteItem() {

    this.apiService.deleteExternalDashboard(this.editObject.id,  this.currentRow.Id).subscribe(
      result => {
        for (let i = 0; i < this.tableConfig.data.length; i++) {
          if (this.tableConfig.data[i].Id === this.currentRow.Id) {
            this.tableConfig.data.splice(i, 1);
            this.updateTableContent(this.tableConfig);
          }
        }

        this.sharedService.publishPopupWindowMsg({
          'type': 'feedback',
          'id': 'deleteDashboardSuccess'
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
      'id': 'deleteDashboard',
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
        'Name': this.detailInfo.name,
        'URL': this.detailInfo.url,
        'Ordering': this.detailInfo.ordering,
      };

      // Create new item
      if (this.editObject.isNew) {

        this.apiService.addExternalDashboard(clientId, updateObj).subscribe(
          result => {
            this.tableConfig.data[0]['No'] = this.tableConfig.data.length;
            this.tableConfig.data[0]['Name'] = updateObj.Name;
            this.tableConfig.data[0]['URL'] = updateObj.URL;
            this.tableConfig.data[0]['Ordering'] = updateObj.Ordering;
            this.tableConfig.data[0]['Id'] = result.Id;
            delete this.tableConfig.data[0].new;
            this.updateTableContent(this.tableConfig);
            this.cancelNewPanel();
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'createDashboardSuccess'
            });
          },
          error => {
              console.error(error);
          }
        );

      // Edit specific item
      }else {
        this.apiService.editExternalDashboardInfo(clientId, this.currentRow.Id, updateObj).subscribe(
          result => {
            for (let i = 0; i < this.tableConfig.data.length; i++) {
              if (this.tableConfig.data[i].Id === this.currentRow.Id) {
                this.tableConfig.data[i]['Name'] = updateObj.Name;
                this.tableConfig.data[i]['URL'] = updateObj.URL;
                this.tableConfig.data[i]['Ordering'] = updateObj.Ordering;
                this.updateTableContent(this.tableConfig);
              }
            }
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'editDashboardSuccess'
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

    this.editObject.isNew = true;
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
          console.log(commandObject);
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

  ngOnInit() {
    this.apiService.getExternalDashboardList(this.editObject.id).subscribe(
      result => {
        for (let i = 0; i < result.length; i++) {
          this.tableConfig.data.push({
            'No': (i + 1),
            'Name': result[i].Name,
            'URL': result[i].URL,
            'Ordering': result[i].Ordering,
            'Id': result[i].Id
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

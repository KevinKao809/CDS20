import { Component, OnInit } from '@angular/core';
import { CdsApiService } from '../../../../../service/cds-api.service';
import { DataShareService} from '../../../../../service/data-share.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';
class EditInputInfo {
  name: String;
 key: number;
 photoURL: String;
 allowMultipleAppearOnBoard: Boolean;
  constructor(name: String = null, key: number = null, photoURL: String = null, allowMultipleAppearOnBoard: Boolean = true) {
    this.name = name;
    this.key = key;
    this.photoURL = photoURL;
    this.allowMultipleAppearOnBoard = allowMultipleAppearOnBoard;
  }
}
@Component({
  selector: 'app-widget-company',
  templateUrl: './widget-company.component.html',
  styleUrls: ['./widget-company.component.scss']
})
export class WidgetCompanyComponent implements OnInit {
  // Table layout config
  tableConfig = {
    'column': ['No', 'Name', 'Widget Key', 'Active'],
    'columnClass': [10, 30, 30, 30],
    'data': [],
    'canExpand': true,
    'expandTrigger': 'row'
  };
  // detail info (binding to input value in template)
  detailInfo: any;

  // unchanged detail info - use to undo edit after input change
  orginalDetailInfo: any;

  // reset passwork status
  resetPasswordStatus: Boolean = false;

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

    this.apiService.getWidgetKeyInfo(parseInt(selectRowInfo['Widget Key'],10)).subscribe(
      result => {
        this.detailInfo = new EditInputInfo(result.Name, result.Key,result.PhotoURL,result.AllowMultipleAppearOnBoard);
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
    this.resetPasswordStatus = false;
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

    this.apiService.deleteWidgetKeyInfo(this.currentRow['Widget Key']).subscribe(
      result => {
        for (let i = 0; i < this.tableConfig.data.length; i++) {
          if (this.tableConfig.data[i]['Widget Key'] === this.currentRow['Widget Key']) {
            this.tableConfig.data.splice(i, 1);
            this.updateTableContent(this.tableConfig);
          }
        }
        this.sharedService.publishPopupWindowMsg({
          'type': 'feedback',
          'id': 'deleteWidgetCompanySuccess'
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
      'id': 'deleteWidgetCompany',
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
        'Key': this.detailInfo.key,
        'Level':"company",
        'PhotoURL': this.detailInfo.photoURL,
        'AllowMultipleAppearOnBoard':this.detailInfo.allowMultipleAppearOnBoard,

      };

      // Create new Item
      if (this.toTableCommamd.isNew) {

        this.apiService.addWidgetInfo(updateObj).subscribe(
          result => {
            this.tableConfig.data[0]['No'] = this.tableConfig.data.length;
            this.tableConfig.data[0]['Name'] = updateObj.Name;
            this.tableConfig.data[0]['Widget Key'] = updateObj.Key;
            this.tableConfig.data[0]['PhotoURL'] = updateObj.PhotoURL;
            this.tableConfig.data[0]['AllowMultipleAppearOnBoard']=updateObj.AllowMultipleAppearOnBoard;
            delete this.tableConfig.data[0].new;
            this.updateTableContent(this.tableConfig);
            this.cancelNewPanel();
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'createWidgetCompanySuccess'
            });
          },
          error => {
              console.error(error);
          }
        );

      // Edit specific Item
      }else {

        this.apiService.editWidgetKeyInfo(this.currentRow['Widget Key'], updateObj).subscribe(
          result => {
            for (let i = 0; i < this.tableConfig.data.length; i++) {
              if (this.tableConfig.data[i]['Widget Key'] === this.currentRow['Widget Key']) {
                this.tableConfig.data[i]['Name'] = updateObj.Name;
                this.tableConfig.data[i]['Widget Key'] = updateObj.Key;
                this.tableConfig.data[i]['PhotoURL'] = updateObj.PhotoURL;
                this.tableConfig.data[i]['AllowMultipleAppearOnBoard']=updateObj.AllowMultipleAppearOnBoard;
                this.updateTableContent(this.tableConfig);
              }
            }
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'editWidgetCompanySuccess'
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

  ngOnInit() {
    // get List Info
    this.apiService.getWigetClassList("company").subscribe(
      result => {
        for (let i = 0; i < result.length; i++) {
          this.tableConfig.data.push({
            'No': (i + 1),
            'Name': result[i].Name,
            'Widget Key': result[i].Key,
            //'Active': result[i].Active
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

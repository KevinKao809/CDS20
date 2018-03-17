import { Component, OnInit, Input, EventEmitter} from '@angular/core';
import { CdsApiService } from '../../../../service/cds-api.service';
import { DataShareService} from '../../../../service/data-share.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';
import { TransferTimeService } from '../../../../service/transfer-time.service';

class EditInputInfo {
  firstName: String;
  lastName: String;
  email: String;
  oldPassword: String;
  password: String;
  confirmPassword: String;
  deletedFlag: Boolean;
  id: String;

  constructor(firstName: String = null, lastName: String = null, email: String = null, oldpassword: String = null, password: String = null,
              confirmPassword: String = null, deletedFlag: Boolean = false, id: String = null) {
    this.firstName = firstName;
    this.lastName = lastName;
    this.email = email;
    this.oldPassword = oldpassword;
    this.password = password;
    this.confirmPassword = confirmPassword;
    this.deletedFlag = deletedFlag;
    this.id = id;
  }
}

@Component({
  templateUrl: './super-admin.component.html',
  styleUrls: ['./super-admin.component.scss'],
  providers: [ TransferTimeService]
})
export class SuperAdminComponent implements OnInit {

  // Table layout config
  tableConfig = {
    'column': ['No', 'First Name', 'Last Name', 'E-mail', 'Created At', 'Active'],
    'columnClass': [10, 20, 20, 20, 20, 10],
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

  constructor(private apiService: CdsApiService, private sharedService: DataShareService,
              private transferTimeService: TransferTimeService) {
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

    this.apiService.getSuperAdminInfo(parseInt(selectRowInfo.Id, 10) ).subscribe(
      result => {
        console.log(result);
        this.detailInfo = new EditInputInfo(result.FirstName,
                                            result.LastName,
                                            result.Email,
                                            null,
                                            null,
                                            null,
                                            result.DeletedFlag,
                                            result.Id);
          this.orginalDetailInfo = Object.assign({}, this.detailInfo);

          console.log(this.orginalDetailInfo);
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
    this.resetPasswordStatus = false;
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

    this.apiService.deleteSuperAdmin(this.currentRow.Id).subscribe(
      result => {
        for (let i = 0; i < this.tableConfig.data.length; i++) {
          if (this.tableConfig.data[i].Id === this.currentRow.Id) {
            this.tableConfig.data.splice(i, 1);
            this.updateTableContent(this.tableConfig);
          }
        }

        this.sharedService.publishPopupWindowMsg({
          'type': 'feedback',
          'id': 'deleteSuperAdminSuccess'
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
      'id': 'deleteSuperAdmin',
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
        'FirstName': this.detailInfo.firstName,
        'LastName': this.detailInfo.lastName,
        'Email': this.detailInfo.email,
        'password': this.detailInfo.password,
        'DeletedFlag': false
      };
      const updatePassword = {
        'OldPassword': this.detailInfo.oldpassword,
        'NewPassword': this.detailInfo.confirmPassword
      };
      // Create new Item
      if (this.toTableCommamd.isNew) {

        const statusUI = '<div><div class="status-dot dot-active"></div><span>Active</span></div>';

        this.apiService.addSuperAdmin(updateObj).subscribe(
          result => {
            this.tableConfig.data[0]['No'] = this.tableConfig.data.length;
            this.tableConfig.data[0]['E-mail'] = updateObj.Email;
            this.tableConfig.data[0]['First Name'] = updateObj.FirstName;
            this.tableConfig.data[0]['Last Name'] = updateObj.LastName;
            this.tableConfig.data[0]['Active'] = statusUI;
            this.tableConfig.data[0]['Created At'] = '-';
            this.tableConfig.data[0]['Id'] = result.Id;
            delete this.tableConfig.data[0].new;
            this.updateTableContent(this.tableConfig);
            this.cancelNewPanel();
            this.sharedService.publishPopupWindowMsg({
              'type': 'feedback',
              'id': 'createSuperAdminSuccess'
            });
          },
          error => {
              console.error(error);
          }
        );

      // Edit specific Item
      }else {

        console.log(this.detailInfo.deletedFlag);

        updateObj.DeletedFlag = this.detailInfo.deletedFlag;
        const updateModifyTableValue = () => {
          for (let i = 0; i < this.tableConfig.data.length; i++) {
            if (this.tableConfig.data[i].Id === this.currentRow.Id) {

              const statusUI = updateObj.DeletedFlag ? '<div><div class="status-dot dot-warning"></div><span>Inactive</span></div>' :
                                                       '<div><div class="status-dot dot-active"></div><span>Active</span></div>';

              this.tableConfig.data[i]['E-mail'] = updateObj.Email;
              this.tableConfig.data[i]['First Name'] = updateObj.FirstName;
              this.tableConfig.data[i]['Last Name'] = updateObj.LastName;
              this.tableConfig.data[i]['Active'] = statusUI;
              this.updateTableContent(this.tableConfig);
            }
          }
          this.sharedService.publishPopupWindowMsg({
            'type': 'feedback',
            'id': 'editSuperAdminSuccess'
          });
        };

        // change password
        if (this.resetPasswordStatus) {

          this.apiService.parallelAsyncRequest([
            this.apiService.editSuperAdminPassword(this.currentRow.Id, updatePassword),
            this.apiService.editSuperAdmin(this.currentRow.Id, updateObj)
          ]).subscribe(
            result => {
              updateModifyTableValue();
            },
            error => {
              console.log('error');
            }
          );

        // not change password
        }else {
           this.apiService.editSuperAdmin(this.currentRow.Id, updateObj).subscribe(
            result => {
              updateModifyTableValue();
            },
            error => {
              console.log('error');
            }
           );
        }
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
    // get SuperAdmin Info
    this.apiService.getSuperAdminList().subscribe(
      result => {
        for (let i = 0; i < result.length; i++) {
          const statusUI = result[i].DeletedFlag ? '<div><div class="status-dot dot-warning"></div><span>Inactive</span></div>' :
                                                  '<div><div class="status-dot dot-active"></div><span>Active</span></div>';

          this.tableConfig.data.push({
            'Id': result[i].Id,
            'No': (i + 1),
            'First Name': result[i].FirstName,
            'Last Name': result[i].LastName,
            'E-mail': result[i].Email,
            'Created At': this.transferTimeService.transferUTCTimeToLocalTime(result[i].CreatedAt, true),
            'Active': statusUI,
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

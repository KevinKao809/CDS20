import { Component, OnInit, OnDestroy, Input, EventEmitter } from '@angular/core';
import { CdsApiService } from '../../../../service/cds-api.service';
import { DataShareService} from '../../../../service/data-share.service';
import { NgForm } from '@angular/forms/src/directives/ng_form';
import { Subscription } from 'rxjs/subscription';
import 'rxjs/add/operator/skip';

class EditInputInfo {
  photoURL: String;
  email: String;
  employeeNumber: String;
  firstName: String;
  lastName: String;
  isAdmin: Boolean;
  password: String;
  confirmPassword: String;
  id: String;

  constructor(photoURL: String = null, email: String = null, employeeNumber: String = null, firstName: String = null,
              lastName: String = null, isAdmin: Boolean = false, password: String = null, confirmPassword: String = null,
              id: String = null) {
    this.photoURL = photoURL;
    this.email = email;
    this.employeeNumber = employeeNumber;
    this.firstName = firstName;
    this.lastName = lastName;
    this.isAdmin = isAdmin;
    this.password = password;
    this.confirmPassword = confirmPassword;
    this.id = id;
  }
}

@Component({
  selector: 'app-edit-employee',
  templateUrl: './edit-employee.component.html',
  styleUrls: ['./edit-employee.component.scss'],
})
export class EditEmployeeComponent implements OnInit, OnDestroy {

  @Input() editObject: any;

  tableConfig = {
    'column': ['No', 'Employee Number', 'First Name', 'Last Name', 'E-mail', 'isAdmin'],
    'columnClass': [10, 20, 20, 20, 20, 10],
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

  uploadImage: any = null;

  private cropImageResultSubscription: Subscription;

  constructor(private apiService: CdsApiService, private sharedService: DataShareService) {
    this.detailInfo = new EditInputInfo();
  }

  getCropFile(cropImage) {
    this.detailInfo.photoURL = cropImage.url;
    this.uploadImage = cropImage.file;
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
    this.uploadImage = null;
    this.apiService.getEmployeeInfo(parseInt(selectRowInfo.Id, 10) ).subscribe(
      result => {
        this.detailInfo = new EditInputInfo(result.PhotoURL, result.Email, result.EmployeeNumber, result.FirstName, result.LastName,
          result.AdminFlag, '', '', result.Id);
          console.log(this.detailInfo);
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
    this.apiService.deleteEmployee(this.currentRow.Id).subscribe(
      result => {
        for (let i = 0; i < this.tableConfig.data.length; i++) {
          if (this.tableConfig.data[i].Id === this.currentRow.Id) {
            this.tableConfig.data.splice(i, 1);
            this.updateTableContent(this.tableConfig);
          }
        }

        this.sharedService.publishPopupWindowMsg({
          'type': 'feedback',
          'id': 'deleteEmployeeSuccess'
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
      'id': 'deleteEmployee',
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
        'EmployeeNumber': this.detailInfo.employeeNumber,
        'Password': this.detailInfo.password,
        'FirstName': this.detailInfo.firstName,
        'LastName': this.detailInfo.lastName,
        'Email': this.detailInfo.email,
        'AdminFlag': this.detailInfo.isAdmin,
        'Lang': 'en',
        'Companyid': clientId
      };
      const updatePassword = {
        'NewPassword': this.detailInfo.confirmPassword
       };
      // Create new employee
      if (this.editObject.isNew) {

        this.apiService.addEmployee(clientId, updateObj).subscribe(
          result => {
            this.tableConfig.data[0]['No'] = this.tableConfig.data.length;
            this.tableConfig.data[0]['E-mail'] = updateObj.Email;
            this.tableConfig.data[0]['Employee Number'] = updateObj.EmployeeNumber;
            this.tableConfig.data[0]['First Name'] = updateObj.FirstName;
            this.tableConfig.data[0]['Last Name'] = updateObj.LastName;
            this.tableConfig.data[0]['isAdmin'] = updateObj.AdminFlag;
            this.tableConfig.data[0]['Id'] = result.Id;
            delete this.tableConfig.data[0].new;
            this.updateTableContent(this.tableConfig);
            this.cancelNewPanel();

            if (this.uploadImage === null) {

              this.sharedService.publishPopupWindowMsg({
                'type': 'feedback',
                'id': 'createEmployeeSuccess'
              });

            }else {

              this.apiService.uploadEmployeeImage(result.Id, this.uploadImage).subscribe(
                image => {
                  this.sharedService.publishPopupWindowMsg({
                    'type': 'feedback',
                    'id': 'createEmployeeSuccess'
                  });
                },
                error => {
                    console.error(error);
                }
              );

            }


          },
          error => {
              console.error(error);
          }
        );

      // Edit specific employee
      }else {

        const updateTableView = () => {
          for (let i = 0; i < this.tableConfig.data.length; i++) {
            if (this.tableConfig.data[i].Id === this.currentRow.Id) {
              this.tableConfig.data[i]['E-mail'] = updateObj.Email;
              this.tableConfig.data[i]['Employee Number'] = updateObj.EmployeeNumber;
              this.tableConfig.data[i]['First Name'] = updateObj.FirstName;
              this.tableConfig.data[i]['Last Name'] = updateObj.LastName;
              this.tableConfig.data[i]['isAdmin'] = updateObj.AdminFlag;
              this.updateTableContent(this.tableConfig);
            }
          }
          this.sharedService.publishPopupWindowMsg({
            'type': 'feedback',
            'id': 'editEmployeeSuccess'
          });
        };

        if (this.uploadImage === null && this.resetPasswordStatus === false) {
          this.apiService.editEmployeeInfo(this.currentRow.Id, updateObj).subscribe(
            result => {
              updateTableView();
            },
            error => {
                console.error(error);
            }
          );

        }else if (this.uploadImage) {

          this.apiService.parallelAsyncRequest([
            this.apiService.uploadEmployeeImage(this.currentRow.Id, this.uploadImage),
            this.apiService.editEmployeeInfo(this.currentRow.Id, updateObj),
          ]).subscribe(
            result => {
              updateTableView();
            },
            error => {
              console.error(error);
            }
          );

        } else if (this.resetPasswordStatus) {
          this.apiService.parallelAsyncRequest([
            this.apiService.editEmployeeInfo(this.currentRow.Id, updateObj),
            this.apiService.editResetEmployeePassword(this.currentRow.Id, updatePassword),
          ]).subscribe(
            result => {
              updateTableView();
            },
            error => {
              console.error(error);
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
    this.uploadImage = null;
    this.editObject.isNew = true;
    this.detailInfo = new EditInputInfo();
    this.tableConfig.data.unshift(
      {
        'email': '',
        'employeeNumber': '',
        'firstName': '',
        'lastName': '',
        'isAdmin': false,
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

        case 2 : // cancel add employee
          this.tableConfig.data.shift();
          this.cancelNewPanel();
          break;

        default:

      }

    }

  }

  /**
   * imageChange: Upload image change
   * @param event : input event
   * @param editForm : form status
   */
  imageChange(event, editForm: NgForm) {

    editForm.form.markAsDirty();

      this.sharedService.publishCropImage({
        'type': 'cropImage',
        'imageChangeEvent': event,
      });
  }

  ngOnDestroy() {
    this.cropImageResultSubscription.unsubscribe();
  }

  ngOnInit() {

    // subscribe to crop image
    this.cropImageResultSubscription = this.sharedService.cropImageModalResultMsg$.skip(1).subscribe(cropImageMsgResult => {
      this.getCropFile(cropImageMsgResult);
    });

    this.apiService.getEmployeeList(this.editObject.id).subscribe(
      employeeList => {
        for (let i = 0; i < employeeList.length; i++) {
          this.tableConfig.data.push({
            'No': (i + 1),
            'Employee Number': employeeList[i].EmployeeNumber,
            'First Name': employeeList[i].FirstName,
            'Last Name': employeeList[i].LastName,
            'E-mail': employeeList[i].Email,
            'isAdmin': employeeList[i].AdminFlag,
            'Id': employeeList[i].Id
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

import { CdsApiService } from './../../../../service/cds-api.service';
import { Component, OnInit, OnDestroy, Input, Output, EventEmitter, ViewChild} from '@angular/core';
import { NgForm } from '@angular/forms/src/directives/ng_form';
import { DataShareService } from '../../../../service/data-share.service';
import { Subscription } from 'rxjs/subscription';
import 'rxjs/add/operator/skip';

/**
 * Use to create uuid
 */
class Guid {
  static newGuid() {
      return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
          const r = Math.random()*16|0, v = c === 'x' ? r : (r&0x3|0x8);
          return v.toString(16);
      });
  }
}

class EditInputInfo {
  logoURL: String;
  name: String;
  shortName: String;
  address: String;
  latitude: Number;
  longitude: Number;
  contact: String;
  phone: String;
  email: String;
  website: String;
  allowDomain: String;
  cultureInfoName: String;
  cultureInfoId: String;
  extAppKey: String;
  id: String;

  constructor(logoURL: String = null, name: String = null, shortName: String = null, address: String = null, latitude: Number = null,
    longitude: Number = null, contact: String = null, phone: String = null, email: String = null,
    website: String = null, allowDomain: String = null, cultureInfoName: String = null, cultureInfoId: String = null,
    extAppKey: String = null, id: String = null) {

      this.logoURL = logoURL;
      this.name = name;
      this.shortName = shortName;
      this.address = address;
      this.latitude = latitude;
      this.longitude = longitude;
      this.contact = contact;
      this.phone = phone;
      this.email = email;
      this.website = website;
      this.allowDomain = allowDomain;
      this.cultureInfoName = cultureInfoName;
      this.cultureInfoId = cultureInfoId;
      this.extAppKey = extAppKey;
      this.id = id;
    }
}

@Component({
  selector: 'app-edit-clients',
  templateUrl: './edit-clients.component.html',
  styleUrls: ['./edit-clients.component.scss']
})
export class EditClientsComponent implements OnInit, OnDestroy {

  @Input() editObject;
  @Input() tableObject;
  @Input() refCultureList;
  @Output() toClientCommand = new EventEmitter();

  private cropImageResultSubscription: Subscription;

  detailInfo: any;
  orginalDetailInfo: any;
  extKeyisCopy: Boolean = false;
  uploadImage: any = null;

  constructor(private apiService: CdsApiService, private dataShareService: DataShareService) {
    this.detailInfo = new EditInputInfo();
  }

  getCropFile(cropImage) {
    this.detailInfo.logoURL = cropImage.url;
    this.uploadImage = cropImage.file;
  }

   // change image
   imageChange(event, editForm: NgForm) {

      editForm.form.markAsDirty();

      this.dataShareService.publishCropImage({
        'type': 'cropImage',
        'imageChangeEvent': event,
      });
  }

  /**
   * deleteItem: Delete specific Item
   */
  deleteItem() {
    this.apiService.deleteCompany(parseInt(this.detailInfo.id, 10)).subscribe(
      result => {
        for (let i = 0; i < this.tableObject.data.length; i++) {
          if (this.tableObject.data[i].Id === this.detailInfo.id) {
            this.tableObject.data.splice(i, 1);
            break;
          }
        }
        this.toClientCommand.emit({
          commandType: 'clientCommand',
          command: 2,
          data: this.tableObject
        });
      },
      error => {

      }
    );
  }

  /**
   * cancelNewPanel: Cancel add client panel
   */
  cancelNewPanel() {
    this.toClientCommand.emit({
      commandType: 'clientCommand',
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
      'id': 'deleteClient',
      'position': {x: posX + 255, y: posY},
      'confirmCallback': () => this.deleteItem(),
      'cancelCallback': () => null
    });
  }


  createBackendCosmosDBCollection(clientId) {
    // this.apiService.addCosmosDBCollection(clientId).subscribe(
    //   result => {
    //     console.log('success');
    //   },
    //   error => {

    //   }
    // );
  }

  /**
   * confirmEditClients: Save edit clients info
   * param editForm : form infomation
   */
  confirmEdit(editForm: NgForm) {

    // if form valid
    if (editForm.valid) {
      const updateClientsObject = {
        'name': this.detailInfo.name,
        'shortname': this.detailInfo.shortName,
        'address': this.detailInfo.address,
        'companywebsite': this.detailInfo.website,
        'contactname': this.detailInfo.contact,
        'contactphone': this.detailInfo.phone,
        'contactemail': this.detailInfo.email,
        'latitude': this.detailInfo.latitude,
        'Longitude': this.detailInfo.longitude,
        'CultureInfoId': this.detailInfo.cultureInfoId,
        'AllowDomain': this.detailInfo.allowDomain,
        'ExtAppAuthenticationKey': this.detailInfo.extAppKey
      };

      // Add New Clinet
      if (this.editObject.isNew) {

        this.apiService.addCompany(updateClientsObject).subscribe(
          result => {

            this.createBackendCosmosDBCollection(result.Id);

            this.tableObject.data[0].No = this.tableObject.data.length;
            this.tableObject.data[0].Id = result.Id;
            this.tableObject.data[0].Name = updateClientsObject.name;
            this.tableObject.data[0].ShortName = updateClientsObject.shortname;
            this.tableObject.data[0].Contact = updateClientsObject.contactname;

            if (this.uploadImage === null) {
              this.toClientCommand.emit({
                commandType: 'clientCommand',
                command: 0
              });
            }else {

              this.apiService.uploadCompanyImage(result.Id, this.uploadImage).subscribe(
                image => {
                  this.toClientCommand.emit({
                    commandType: 'clientCommand',
                    command: 0
                  });
                },
                error => {
                  console.log('upload error');
                }
              );
            }
          },
          error => {
            console.log('error');
          }
        );

      // Edit Client
      } else {

        const updateModifyTableValue  = () => {
          for (let i = 0; i < this.tableObject.data.length; i++) {
            if (this.tableObject.data[i].Id === this.detailInfo.id) {
              this.tableObject.data[i].Name = this.detailInfo.name;
              this.tableObject.data[i].ShortName = this.detailInfo.shortName;
              this.tableObject.data[i].Contact = this.detailInfo.contact;
              break;
            }
          }

          this.toClientCommand.emit({
            commandType: 'clientCommand',
            command: 1,
            data: this.tableObject
          });

        };

        // do upload image
        if (this.uploadImage !== null) {
           this.apiService.parallelAsyncRequest([
            this.apiService.uploadCompanyImage(this.detailInfo.id, this.uploadImage),
            this.apiService.editCompanyInfo(this.detailInfo.id, updateClientsObject),
           ]).subscribe(
            result => {
              updateModifyTableValue();
            },
            error => {
              console.log('error');
            }
          );

        // not upload image
        }else {

          this.apiService.editCompanyInfo(parseInt(this.detailInfo.id, 10), updateClientsObject).subscribe(
            result => {
              updateModifyTableValue();
            },
            error => {

            }
          );
        }
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

  /**
   * cultureChange: change select culture info
   * param selectCultureInfoId: culture id
   */
  cultureChange(selectCultureInfoId) {
    this.detailInfo.cultureInfoId = selectCultureInfoId;
  }

  /**
   * To Creat External auth key
   */
  createExternalKey(editForm: NgForm) {
    const uuid = Guid.newGuid();
    const extKey = btoa(uuid);
    this.detailInfo.extAppKey = extKey;
    editForm.form.markAsDirty();
  }

  /**
   * To Clear External auth key
   */
  clearExternalKey(editForm: NgForm) {
    editForm.form.markAsDirty();
    this.detailInfo.extAppKey = '';
  }

  /**
   * Copy Ext. app. key success function
   */
  copySuccess() {
    this.extKeyisCopy = true;
    setTimeout(() => this.extKeyisCopy = false, '1000');
  }

  ngOnDestroy() {
    this.cropImageResultSubscription.unsubscribe();
  }

  ngOnInit() {
    this.uploadImage = null;

    // subscribe to crop image
    this.cropImageResultSubscription = this.dataShareService.cropImageModalResultMsg$.skip(1).subscribe(cropImageMsgResult => {
      this.getCropFile(cropImageMsgResult);
    });

    if (!this.editObject.isNew) {
      this.apiService.getCompanyInfo(this.editObject.id).subscribe(
        result => {

          this.detailInfo = new EditInputInfo(result.LogoURL, result.Name, result.ShortName, result.Address,
            result.Latitude, result.Longitude, result.ContactName, result.ContactPhone, result.ContactEmail,
            result.CompanyWebSite, result.AllowDomain, result.CultureInfoName, result.CultureInfoId, result.ExtAppAuthenticationKey,
            result.Id);

          this.orginalDetailInfo = Object.assign({}, this.detailInfo);
        },
        error => {
            console.error(error);
        }
      );
    }
  }
}

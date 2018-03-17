import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { URLSearchParams } from '@angular/http';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/observable/combineLatest';
import { LoginManagementService } from '../service/login-management.service';
import { DataShareService } from '../service/data-share.service';

@Injectable()
export class CdsApiService {

  private serverUrl: string;
  private actionUrl: string;
  private apiEndPoint = 'admin-api';

  constructor(private http: HttpClient, private loginManagement: LoginManagementService, private dataShareService: DataShareService) {
    console.log(environment.envName);
    this.serverUrl = environment.APIServiceBaseURI;
    this.actionUrl = this.serverUrl + '/' + this.apiEndPoint;
  }

  private errorHanndler(error) {
    this.dataShareService.publishPopupWindowMsg({
      'type': 'APIError',
      'id': 'errorMessage',
      'errorCode': error.status,
      'statusText': error.statusText
    });
  }

  /**
   * refreshTokenLogin: To use refresh Token to get new token
   */
  private refreshTokenLogin(apiObject: any = null): Observable<any> {
    const refreshToken = localStorage.getItem('cds-refresh_token') ? localStorage.getItem('cds-refresh_token') : null;

    if (!refreshToken) {
      this.loginManagement.logOut();
    } else {
      const userSendingObject = {
        'grant_type': 'refresh_token',
        'refresh_token': refreshToken,
        'client_id': 'superadmin',
      };

      return this.http.post(this.serverUrl + '/token', this.convertFormData(userSendingObject), { headers: null })
        .flatMap((reloginInfo: any) => {
          this.loginManagement.login(reloginInfo, true);
          console.log(apiObject);
          const apiData = (apiObject.body) ? this.convertFormData(apiObject.body) : null;
          return this.createHttpRequest(apiObject.method, apiObject.url, apiData);
        })
        .catch(error => {
          this.loginManagement.logOut();
          return Observable.throw(error);
        });
    }
  }

  /**
   *  convertFormData: To convert json object to form data
   */
  private convertFormData(sendDataObject: any) {

    const urlSearchParams = new URLSearchParams();
    const objectLength = Object.keys(sendDataObject).length;

    for (let i = 0; i < objectLength; i++) {
      const key = Object.keys(sendDataObject)[i];
      urlSearchParams.set(key, sendDataObject[key]);
    }
    return urlSearchParams.toString();
  }

  private convertMutipleFormData(sendDataObject: any) {
    const formData = new FormData();
    formData.append('image', sendDataObject, sendDataObject.name);
    return formData;
  }

  /**
   * createHttpRequest : to create a ajax request to cds api
   */
  private createHttpRequest(method, url, body = null, needToken: boolean = true, clearContentType: boolean = false) {

    const apiObject = {
      'method': method,
      'url': url,
      'body': body,
      'needToken': needToken
    };

    let headers = new HttpHeaders();

    if (needToken) {
      const accessToken = localStorage.getItem('cds-token');

      if (!clearContentType) {
        headers = headers.set('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8');
      }

      headers = headers.set('Authorization', 'Bearer ' + accessToken);
    }

    switch (method) {
      case 'get':
      case 'delete':
        return this.http[method](url, { headers: headers }).map(response => {
          return response;
        })
          .catch(error => {
            if (error.status === 401) {
              return this.refreshTokenLogin(apiObject);
            } else {
              this.errorHanndler(error);
              return Observable.throw(error);
            }
          });

      case 'post':
      case 'put':
      case 'patch':
        return this.http[method](url, body, { headers: headers }).map(response => {
          return response;
        })
          .catch(error => {
            if (error.status === 401) {
              return this.refreshTokenLogin(apiObject);
            } else {
              this.errorHanndler(error);
              return Observable.throw(error);
            }
          });

      default:
        break;
    }
  }

  /**
   * parallelAsyncRequest: Combine api request
   */
  parallelAsyncRequest(apiArray) {
    const combined = Observable.combineLatest(apiArray);

    return combined.map(
      result => {
        return result;
      },
      error => {
        return Observable.throw(error);
      }
    );
  }

  // -------------- API Service  ----------------- //

  // Get Auth Token
  getToken(data) {
    const url = this.serverUrl + '/token';
    return this.createHttpRequest('post', url, this.convertFormData(data), false);
  }

  // Get RefCultureInfo
  getRefCultureInfo() {
    const url = this.actionUrl + '/RefCultureInfo';
    return this.createHttpRequest('get', url);
  }

  // Add Company
  addCompany(data) {
    const url = this.actionUrl + '/Company';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }

  // Get Company List
  getCompanyList() {
    const url = this.actionUrl + '/Company';
    return this.createHttpRequest('get', url);
  }

  // Get One Company Info By ID
  getCompanyInfo(id: number) {
    const url = this.actionUrl + '/Company/' + id;
    return this.createHttpRequest('get', url);
  }

  // Upload Company Logo
  uploadCompanyImage(id: number, file) {
    const url = this.actionUrl + '/Company/' + id + '/Image';
    return this.createHttpRequest('put', url, this.convertMutipleFormData(file), true, true);
  }

  // Delete Company By ID
  deleteCompany(id: number) {
    const url = this.actionUrl + '/Company/' + id;
    return this.createHttpRequest('delete', url);
  }

  // Get One Company's Emplyee List By ID
  getEmployeeList(id: number) {
    const url = this.actionUrl + '/Company/' + id + '/employee';
    return this.createHttpRequest('get', url);
  }

  // editCompanyInfo
  editCompanyInfo(id: number, data) {
    const url = this.actionUrl + '/Company/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }

  // Get one Employee's Info
  getEmployeeInfo(id: number) {
    const url = this.actionUrl + '/Employee/' + id;
    return this.createHttpRequest('get', url);
  }

  // add employee
  addEmployee(id: number, data) {
    const url = this.actionUrl + '/Company/' + id + '/Employee';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }

  // delete employee
  deleteEmployee(id: number) {
    const url = this.actionUrl + '/Employee/' + id;
    return this.createHttpRequest('delete', url);
  }

  // Upload employee image
  uploadEmployeeImage(id: number, file) {
    const url = this.actionUrl + '/Employee/' + id + '/Image';
    return this.createHttpRequest('put', url, this.convertMutipleFormData(file), true, true);
  }

  // Edit EmployeeInfo
  editEmployeeInfo(id: number, data) {
    const url = this.actionUrl + '/Employee/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }
  // Edit reset Employee password
  editResetEmployeePassword(id: number, data) {
    const url = this.actionUrl + '/Employee/' + id + '/ResetPassword';
    return this.createHttpRequest('put', url, this.convertFormData(data));
  }
  // Get Iot Hub List
  getIotHubList(id: number) {
    const url = this.actionUrl + '/Company/' + id + '/IoTHub';
    return this.createHttpRequest('get', url);
  }

  // Add Iot Hub
  addIotHub(id: number, data) {
    const url = this.actionUrl + '/Company/' + id + '/IoTHub';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }

  // Get one Iot Hub's Info
  getIotHubInfo(id: number) {
    const url = this.actionUrl + '/IoTHub/' + id;
    return this.createHttpRequest('get', url);
  }

  // Edit Iot Hub
  editIotHubInfo(id: number, data) {
    const url = this.actionUrl + '/IoTHub/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }

  // Delete Iot Hub
  deleteIotHub(id: number) {
    const url = this.actionUrl + '/IoTHub/' + id;
    return this.createHttpRequest('delete', url);
  }

  // Get External Dashboard list
  getExternalDashboardList(id: number) {
    const url = this.actionUrl + '/Company/' + id + '/ExternalDashboard';
    return this.createHttpRequest('get', url);
  }

  // Get One External Dashboard Info
  getExternalDashboardInfo(companyId: number, dashboardId: number) {
    const url = this.actionUrl + '/Company/' + companyId + '/ExternalDashboard/' + dashboardId;
    return this.createHttpRequest('get', url);
  }

  // Add External Dashboard
  addExternalDashboard(id: number, data: {}) {
    const url = this.actionUrl + '/Company/' + id + '/ExternalDashboard';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }

  // Edit External Dashboard Info
  editExternalDashboardInfo(companyId: number, dashboardId: number, data: {}) {
    const url = this.actionUrl + '/Company/' + companyId + '/ExternalDashboard/' + dashboardId;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }

  // Delete External Dashboard
  deleteExternalDashboard(companyId: number, dashboardId: number) {
    const url = this.actionUrl + '/Company/' + companyId + '/ExternalDashboard/' + dashboardId;
    return this.createHttpRequest('delete', url);
  }

  // Get Subscription List Of Specific Company
  getCompanySubscriptionList(id: number) {
    const url = this.actionUrl + '/Company/' + id + '/Subscription';
    return this.createHttpRequest('get', url);
  }

  // Get One Subscription Info Of Specific Company
  getCompanySubscriptionInfo(companyId: number, subscriptionId: number) {
    const url = this.actionUrl + '/Company/' + companyId + '/Subscription/' + subscriptionId;
    return this.createHttpRequest('get', url);
  }

  // Add company subscription plan
  addCompanySubscriptionPlan(companyId: number, data: {}) {
    const url = this.actionUrl + '/Company/' + companyId + '/Subscription';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }

  // Edit company subscription plan
  editCompanySubscriptionPlan(companyId: number, subscriptionId: number, data: {}) {
    const url = this.actionUrl + '/Company/' + companyId + '/Subscription/' + subscriptionId;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }

  // Unsubscript company subscription plan
  cancelCompanySubscriptionPlan(companyId: number, subscriptionId: number) {
    const url = this.actionUrl + '/Company/' + companyId + '/Subscription/' + subscriptionId;
    return this.createHttpRequest('delete', url);
  }

  // Get All Subscription List
  getAllSubscriptionPlanList() {
    const url = this.actionUrl + '/SubscriptionPlan';
    return this.createHttpRequest('get', url);
  }
  // Get Subscription detail
  getSubscriptionPlan(id: number) {
    const url = this.actionUrl + '/SubscriptionPlan/' + id;
    return this.createHttpRequest('get', url);
  }
  // Add Subscription detail
  addSubscriptionPlan(data: {}) {
    const url = this.actionUrl + '/SubscriptionPlan';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }
  // Edit Subscription detail
  editSubscriptionPlan(id: number, data: {}) {
    const url = this.actionUrl + '/SubscriptionPlan/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }
  // Delete Subscription detail
  deleteSubscription(id: number) {
    const url = this.actionUrl + '/SubscriptionPlan/' + id;
    return this.createHttpRequest('delete', url);
  }

  // Get SuperAdmin List
  getSuperAdminList() {
    const url = this.actionUrl + '/SuperAdmin';
    return this.createHttpRequest('get', url);
  }

  // Get SuperAdmin Info
  getSuperAdminInfo(id: number) {
    const url = this.actionUrl + '/SuperAdmin/' + id;
    return this.createHttpRequest('get', url);
  }

  // Add SuperAdmin Info
  addSuperAdmin(data: {}) {
    const url = this.actionUrl + '/SuperAdmin/';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }

  // Edit SuperAdmin Info
  editSuperAdmin(id: number, data: {}) {
    const url = this.actionUrl + '/SuperAdmin/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }
  // Change SuperAdmin Password
  editSuperAdminPassword(id: number, data: {}) {
    const url = this.actionUrl + '/SuperAdmin/' +  id + '/ChangePassword';
    return this.createHttpRequest('put' , url, this.convertFormData(data));
  }

  // Delete SuperAdmin
  deleteSuperAdmin(id: number) {
    const url = this.actionUrl + '/SuperAdmin/' + id;
    return this.createHttpRequest('delete', url);
  }

  // Get Permission Catalog List
  getPermissionCatalogList() {
    const url = this.actionUrl + '/PermissionCatalog';
    return this.createHttpRequest('get', url);
  }

  // Get Permission Catalog Info
  getPermissionCatalogInfo(id: number) {
    const url = this.actionUrl + '/PermissionCatalog/' + id;
    return this.createHttpRequest('get', url);
  }

  // Add Permission Catalog
  addPermissionCatalog(data: {}) {
    const url = this.actionUrl + '/PermissionCatalog/';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }

  // Edit Permission Catalog Info
  editPermissionCatalog(id: number, data: {}) {
    const url = this.actionUrl + '/PermissionCatalog/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }

  // Delete Permission Catalog
  deletePermissionCatalog(id: number) {
    const url = this.actionUrl + '/PermissionCatalog/' + id;
    return this.createHttpRequest('delete', url);
  }

  // Get Device Class List
  getDeviceClassList() {
    const url = this.actionUrl + '/DeviceType';
    return this.createHttpRequest('get', url);
  }

  // Get One Device Class Lisr Info
  getDeviceClassInfo(id: number) {
    const url = this.actionUrl + '/DeviceType/' + id;
    return this.createHttpRequest('get', url);
  }

  // Add Device Class
  addDeviceClass(data: {}) {
    const url = this.actionUrl + '/DeviceType';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }

  // Edit Device Class
  editDeviceClass(id: number, data: {}) {
    const url = this.actionUrl + '/DeviceType/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }

  // Delete Device Class
  deleteDeviceClass(id: number) {
    const url = this.actionUrl + '/DeviceType/' + id;
    return this.createHttpRequest('delete', url);
  }

  // Get Equipment Class
  getEquipmentClass() {
    const url = this.actionUrl + '/EquipmentClass';
    return this.createHttpRequest('get', url);
  }
  // Get Equipment Class Info
  getEquipmentClassInfo(id: number) {
    const url = this.actionUrl + '/EquipmentClass/' + id;
    return this.createHttpRequest('get', url);
  }
  // Get Equipment Class Message Catalog
  getEquipmentClassMessageCatalog(id: number) {
    const url = this.actionUrl + '/EquipmentClass/' + id + '/MessageCatalog ';
    return this.createHttpRequest('get', url);
  }
  // Add Equipment Class
  addEquipmentClass(data: {}) {
    const url = this.actionUrl + '/EquipmentClass';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }

  // Edit Equipment Class
  editgetEquipmentClass(id: number, data: {}) {
    const url = this.actionUrl + '/EquipmentClass' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }
  // Delete Equipment Class
  deleteEquipmentClass(id: number) {
    const url = this.actionUrl + '/EquipmentClass' + id;
    return this.createHttpRequest('delete', url);
  }
  // Get Device Configuration List
  getDeviceConfigurationList() {
    const url = this.actionUrl + '/IoTDeviceSystemConfiguration';
    return this.createHttpRequest('get', url);
  }
  // Get Device Configuration
  getDeviceConfiguration(id: number) {
    const url = this.actionUrl + '/IoTDeviceSystemConfiguration/' + id;
    return this.createHttpRequest('get', url);
  }
  // Add Device Configuration
  addDeviceConfiguration(data: {}) {
    const url = this.actionUrl + '/IoTDeviceSystemConfiguration';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }
  // Edit Device Configuration
  editDeviceConfiguration(id: number, data: {}) {
    const url = this.actionUrl + '/IoTDeviceSystemConfiguration/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }
  // Delete Device Configuration
  deleteDeviceConfiguration(id: number) {
    const url = this.actionUrl + '/IoTDeviceSystemConfiguration/' + id;
    return this.createHttpRequest('delete', url);
  }
  // Get Mandatory Message List
  getMandatoryMessageList() {
    const url = this.actionUrl + '/MessageMandatoryElementDef';
    return this.createHttpRequest('get', url);
  }
  // Get Mandatory Message
  getMandatoryMessage(id: number) {
    const url = this.actionUrl + '/MessageMandatoryElementDef/' + id;
    return this.createHttpRequest('get', url);
  }
  // Add Mandatory Message
  addMandatoryMessage(data: {}) {
    const url = this.actionUrl + '/MessageMandatoryElementDef';
    return this.createHttpRequest('post', url, this.convertFormData(data));
  }
  // Edit Mandatory Message
  editMandatoryMessage(id: number, data: {}) {
    const url = this.actionUrl + '/MessageMandatoryElementDef/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }
  // Delete Mandatory Message
  deleteMandatoryMessage(id: number) {
    const url = this.actionUrl + '/MessageMandatoryElementDef/' + id;
    return this.createHttpRequest('delete', url);
  }
  // Get Widget List
  getWigetClassList(level: String) {
    const url = this.actionUrl + '/WidgetClass?=' + level;
    return this.createHttpRequest('get', url);
  }
  // Get Widget Info by Key
  getWidgetKeyInfo(key: number) {
    const url = this.actionUrl + '/WidgetClass/' + key;
    return this.createHttpRequest('get', url);
  }
  // Add Widget Info
  addWidgetInfo(data: {}) {
    const url = this.actionUrl + '/WidgetClass';
    return this.createHttpRequest('post', url , this.convertFormData(data));
  }
  // Edit Widget Info by key
  editWidgetKeyInfo(key: number, data: {}) {
    const url = this.actionUrl + '/WidgetClass/' + key;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }
  // Delete Widget Info by key
  deleteWidgetKeyInfo(key: number) {
    const url = this.actionUrl + '/WidgetClass/' + key;
    return this.createHttpRequest('delete', url);
  }

  // Get System Configuration
  getSystemConfiguration() {
    const url = this.actionUrl + '/SystemConfiguration';
    return this.createHttpRequest('get', url);
  }

  // Edit System Configuration
  editSystemConfiguration(id: number, data: {}) {
    const url = this.actionUrl + '/SystemConfiguration/' + id;
    return this.createHttpRequest('patch', url, this.convertFormData(data));
  }

  // Backend Service---------------------------------------------------------------

  // [background] Create cosmosDB collection
  addCosmosDBCollection(clientId) {
    const url = this.actionUrl + '/BackendTask/Company/' + clientId + '/CosmosDBCollection';
    return this.createHttpRequest('post', url);
  }

}

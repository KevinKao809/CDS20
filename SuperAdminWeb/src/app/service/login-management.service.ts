import { Injectable } from '@angular/core';
import { Router} from '@angular/router';

@Injectable()
export class LoginManagementService {

  public error_text = '';

  constructor(private router: Router) { }

  login(loginInfo, relogin: boolean = false) {

    localStorage.setItem('cds-token', loginInfo.access_token);
    localStorage.setItem('cds-refresh_token', loginInfo.refresh_token);

    const userInfo = {
      'Client_Id': loginInfo.Client_Id,
      'Email': loginInfo.Email,
      'FirstName': loginInfo.FirstName,
      'Id': loginInfo.Id,
      'LastName': loginInfo.LastName,
    };

    localStorage.setItem('cds-userInfo', JSON.stringify(userInfo));

    if (relogin) {
      console.log('Relogin');
      // return
    }else {
      this.router.navigate(['/main']);
    }
  }
  logOut() {
      localStorage.removeItem('cds-token');
      localStorage.removeItem('cds-refresh_token');
      localStorage.removeItem('cds-userInfo');
      this.router.navigate(['/login']);
  }

}

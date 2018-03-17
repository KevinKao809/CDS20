import { Component, OnInit } from '@angular/core';
import { LoginManagementService } from '../service/login-management.service';
import { CdsApiService } from '../service/cds-api.service';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  providers: [LoginManagementService]
})
export class LoginComponent implements OnInit {

  userMail: string;
  userPassword: string;
  cdsVersion: String = '2.0.01';

  loginErrorText = true;

  errorShakeAnimation = {
    'shake' : false
  };

  errorWording = 'Invalid email address or password!';

  constructor(private loginManagementService: LoginManagementService, private apiService: CdsApiService) { }

  ngOnInit() {
  }


  loginErrorMessage() {
    this.loginErrorText = false;
    this.errorShakeAnimation = {
      'shake' : true
    };
    setTimeout(() =>  this.errorShakeAnimation = {
      'shake' : false
    } , 820);
  }

  sendLogin() {

    const userSendingObject = {
      'grant_type': 'password',
      'password': this.userPassword,
      'client_id': 'superadmin',
      'username': this.userMail
    };

    this.apiService.getToken(userSendingObject).subscribe(
      userInfo => {
        this.loginManagementService.login(userInfo);
      },
      error => {
          console.error(error);
          this.loginErrorMessage();
      }
    );
  }
}

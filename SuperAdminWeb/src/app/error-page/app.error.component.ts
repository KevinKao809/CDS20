import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  // selector: 'app-root',
  templateUrl: './app.error.component.html',
  styleUrls: ['./app.error.component.scss']
})
export class AppErrorComponent implements OnInit, OnDestroy {


  sub: any;
  errorId: number;
  errorTitle: string;
  cdsVersion: string;

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.cdsVersion = '2.0.0.1';
    this.sub = this.route.params.subscribe(params => {
      this.errorId = +params['id']; // (+) converts string 'id' to a number
      console.log(this.errorId);

      switch (this.errorId) {
        case 404:
          this.errorTitle = '404 Error. Page Not Found.';
        break;

        default:
        this.errorTitle = '';
        break;
      }

      // this.errorId
       // In a real app: dispatch action to load the details here.
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }


}

import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCount = 0;

  constructor(private spinnerService:NgxSpinnerService) { }

  busy(){
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      bdColor : "rgba(0, 0, 0, 0.8)",
       size : "large",
        color : "#fff",
         type : "square-jelly-box",   
    })
  }

  idle(){
    this.busyRequestCount--;
    if(this.busyRequestCount<=0){
      this.busyRequestCount =0;
      this.spinnerService.hide();
    }
  }
}

import { Injectable } from '@angular/core';
import * as moment from 'moment';

@Injectable()
export class TransferTimeService {

  constructor() { }

  /**
   * transferUTCTimeToLocalTime: Transfer UTC Time to Local Time
   * @param utcTime : utc time
   */
  transferUTCTimeToLocalTime(utcTime, toFormat: Boolean = false, customizedFormat: string = 'YYYY-MM-DD HH:mm') {
    let localTime  = moment.utc(utcTime).toDate().toISOString();
    if (toFormat) {
      localTime = moment(localTime).format(customizedFormat);
    }

    return localTime;
  }

  /**
   *  transferLocalTimeToUTCTime:  Transfer Local Time to UTC Time
   * @param time : Local time
   */
   transferLocalTimeToUTCTime(time) {
      return moment.utc(time).format();
   }

 /**
  * getTime:Get Current time
  * @param time :(optional) time (expired time)
  * @param getUTC:(optional) get UTC time
  */
  getTime(getUTC: Boolean = false, time = null) {
    if (time) {
      return moment(time);
    }
    if (getUTC) {
      return moment.utc();
    }
     return moment();
  }
}

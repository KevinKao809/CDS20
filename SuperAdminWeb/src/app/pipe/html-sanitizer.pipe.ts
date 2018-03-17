import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'sanitizeHtml'
})

export class HtmlSanitizerPipe implements PipeTransform {

  constructor(private _sanitizer: DomSanitizer) {}

  transform(value: any): SafeHtml {
    return this._sanitizer.bypassSecurityTrustHtml(value);
  }
}

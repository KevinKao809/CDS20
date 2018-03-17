import { Component, OnInit, OnChanges, Input, Output, EventEmitter, ViewChild} from '@angular/core';
import { animate, state, style, transition, trigger} from '@angular/animations';
import { DataShareService } from '../../../service/data-share.service';

declare var jquery: any;
declare var $: any;

@Component({
  selector: 'app-expandable-table',
  templateUrl: './expandable-table.component.html',
  styleUrls: ['./expandable-table.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({height: '0px', minHeight: '0', visibility: 'hidden'})),
      state('expanded', style({height: '*', visibility: 'visible'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class ExpandableTableComponent implements OnInit, OnChanges {

  @Input() tableSetting: any;
  @Input() slidePaneltemplate: any; // [Optional] expand panel template passed from parent
  @Input() customizedColumnTemplate: any; // [Optional] Customized column template
  @Input() receivedCommand: any;

  @Output() toParentCommand = new EventEmitter();


  @ViewChild('genericTable') table: any;

  public cellClass = [];

  currentRow: any = {};
  displayEntriesNumber = 10;
  displayedColumns: any;

  rows = [];
  selected = [];
  currentInlineEditing = null;
  inlineUpdateValue: any;

  filteredList = [];

  constructor(private dataShareService: DataShareService) {}

  cancelAddPanelConfirm() {
    if (this.rows[0].new) {
      console.warn('~~~ need warning ~~~~');

      // this.dataShareService.publishPopupWindowMsg({
      //   ''
      // });

      this.toParentCommand.emit({
        commandType: 'tableCommand',
        command: 2
      });
    }
  }

  rowSelect({selected}) {

    // console.log(selected);

    // new panel should be selected
    if (!selected || selected.length === 0 || selected[0].new) {
      return false;
    }

    this.cancelAddPanelConfirm();

    // select other row when "new panel" open
    // if (this.rows[0].new) {

    //   console.warn('~~~ need warning ~~~~');
    //   this.toParentCommand.emit({
    //     commandType: 'tableCommand',
    //     command: 2
    //   });
    // }



    // select self again => unselect
    if (this.currentRow === selected[0]) {
      this.cancelAllRowSelection();
      return false;
    }

    this.table.rowDetail.collapseAllRows();

    this.currentRow = selected[0];


    // select row triggr expand
    if (this.tableSetting.canExpand && this.tableSetting.expandTrigger === 'row') {

      // expand other row when add panel is expand
      if (this.rows[0].new) {
        console.warn('~~~ need warning ~~~~');
        this.toParentCommand.emit({
          commandType: 'tableCommand',
          command: 2
        });
      }

      this.toParentCommand.emit({
        commandType: 'tableCommand',
        command: 1,
        data: this.currentRow
      });

      setTimeout(() => {
        this.toggleExpandRow();
      });

    // select row not trigger expand
    }else if (this.tableSetting.canExpand && this.tableSetting.expandTrigger === 'button') {

      this.toParentCommand.emit({
        commandType: 'tableCommand',
        command: 0,
        data: this.currentRow
      });

    }else if (!this.tableSetting.canExpand && this.tableSetting.inlineEdit) {

      this.inlineUpdateValue = this.currentRow.Value;

      this.currentInlineEditing = this.currentRow.Id;
      this.toParentCommand.emit({
        commandType: 'tableCommand',
        command: 0,
        data: this.currentRow
      });

    }
  }

  cancelAllRowSelection() {
    this.inlineUpdateValue = null;
    this.currentInlineEditing = null;
    this.selected = [];
    this.currentRow = null;
    this.table.rowDetail.collapseAllRows();
  }

  rowSort(r) {
    // return false;
    console.log(r);
  }

  updateFilter(event) {
    const val = event.target.value.toLowerCase();

    // const list = p

    const filteredList = this.filteredList.slice().filter((item: any) => {

      let searchStr = '';

      for (let i = 0; i < this.displayedColumns.length; i++) {
        if (item[this.displayedColumns[i]]) {
          searchStr += (item[this.displayedColumns[i]]).toString().toLowerCase();
        }
      }
      return searchStr.indexOf(val) !== -1 || !val;
    });

    // update the rows
    this.rows = filteredList;
    // Whenever the filter changes, always go back to the first page
    this.table.offset = 0;

  }

  toggleExpandRow() {
    this.table.rowDetail.collapseAllRows();
    this.table.rowDetail.toggleExpandRow(this.currentRow);
  }

  onDetailToggle($event) {
    // console.log($event);
  }


  updateInline(event) {
    this.toParentCommand.emit({
          commandType: 'tableCommand',
          command: 3,
          data: this.inlineUpdateValue
    });
  }

  /**
   * directly update dom without ngchanges (used in realtime update data)
   * @param updateObj : updated data detail
   */
  directUpdateCell(updateObj) {
    $.each( updateObj.update , function( key, value ) {
      $('#' + updateObj.id + '-' + key).html(value);
    });
  }

  ngOnChanges() {

    this.rows = [...this.tableSetting.data];
    this.filteredList = [...this.tableSetting.data];

    if (this.receivedCommand) {

      // collapseAllRow
      switch (this.receivedCommand.type) {

        case 'expandRow':

          if (this.receivedCommand.isNew) {
            this.currentRow = this.rows[0];
          }else {
            this.currentRow = this.receivedCommand.row;
          }
          setTimeout(() => {
            this.toggleExpandRow();
          });

        break;

        case 'collapseAllRow':
          this.table.rowDetail.collapseAllRows();
        break;

        case 'cancelSelect':
          this.cancelAllRowSelection();
        break;

        case 'inlineditUndo':
          this.inlineUpdateValue = this.receivedCommand.originalValue;
        break;

      }

    }

  }

  ngOnInit() {
    this.displayedColumns = this.tableSetting.column;
    this.cellClass = this.tableSetting.columnClass;
  }
}

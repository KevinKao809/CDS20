var allNotifyTable,
    emailNotifyTable,
    smsNotifyTable,
    erpNotifyTable,
    mesNotifyTable;

$(document).ready(function () {
    $('#table-filter-header').data('filter', 'all');

    allNotifyTable = $('#all-notify-data-table').DataTable(
        {
            'autoWidth': false,
            "order": [[2, "desc"]]
        }
    );

    $('#all-notify-data-table_wrapper').addClass('table-show');

    emailNotifyTable = $('#email-notify-data-table').DataTable(
        {
            'autoWidth': false,
            "order": [[4, "desc"]]
        }
    );

    smsNotifyTable = $('#sms-notify-data-table').DataTable(
        {
            'autoWidth': false,
            "order": [[4, "desc"]]
        }
    );

    erpNotifyTable = $('#erp-notify-data-table').DataTable(
        {
            'autoWidth': false,
            "order": [[4, "desc"]]
        }
    );

    mesNotifyTable = $('#mes-notify-data-table').DataTable(
        {
            'autoWidth': false,
            "order": [[4, "desc"]]
        }
    );

    renderNotifyTable.init();
    signalRController.init();
});